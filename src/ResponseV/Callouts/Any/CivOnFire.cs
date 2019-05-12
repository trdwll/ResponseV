using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("CivOnFire", CalloutProbability.VeryHigh)]
    public class CivOnFire : RVCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of a Civilian on Fire";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_CIV_ON_FIRE IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, 0f));

            g_Victims.ForEach(v =>
            {
                v.IsOnFire = true; // Makes the civ unable to be extinguished

                DamagePack.ApplyDamagePack(v, Utils.GetRandValue(
                    DamagePack.Burnt_Ped_0, DamagePack.Burnt_Ped_Head_Torso,
                    DamagePack.Burnt_Ped_Left_Arm, DamagePack.Burnt_Ped_Limbs,
                    DamagePack.Burnt_Ped_Right_Arm), 100, 1);

                if (Utils.GetRandBool())
                {
                    v.Kill();
                }
            });


            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(6000);
                LSPDFR.RequestEMS(g_SpawnPoint);
                LSPDFR.RequestFire(g_SpawnPoint);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 20)
            {
                End();
            }
        }
    }
}