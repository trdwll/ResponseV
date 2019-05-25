using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("DeadBody", CalloutProbability.VeryHigh)]
    internal sealed class DeadBody : CalloutBase
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_DEADBODY)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_DEADBODY, Enums.EResponse.R_CODE2)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("DeadBody: Callout accepted");

            g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360)));
            g_Victims.ForEach(v =>
            {
                if (Native.GetSafeCoordForPed(g_SpawnPoint, out Vector3 pos))
                {
                    v.Position = pos;
                }

                v.ApplyDamagePack(Utils.GetRandValue(
                        DamagePack.BigHitByVehicle, DamagePack.SCR_Torture,
                        DamagePack.Explosion_Large, DamagePack.Burnt_Ped_0,
                        DamagePack.Car_Crash_Light, DamagePack.Car_Crash_Heavy,
                        DamagePack.Fall_Low, DamagePack.Fall,
                        DamagePack.HitByVehicle, DamagePack.BigRunOverByVehicle,
                        DamagePack.RunOverByVehicle
                    ), 100, 1);

                v.Kill();
            });

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(3000);
                LSPDFR.RequestEMS(g_SpawnPoint);
                LSPDFR.RequestBackup(g_SpawnPoint, 1);
            });
            
            Main.g_GameFibers.Add(fiber);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            if (g_bOnScene)
            {
                g_Logger.Log("DeadBody: On scene and requesting coroner (if Arrest Manager is installed) then end call");
                LSPDFR.RequestCoroner(g_SpawnPoint);

                End();
            }
        }
    }
}
