using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("BarFight", CalloutProbability.Medium)]
    internal sealed class BarFight : CalloutBase
    {
        private Ped m_Suspect1;
        private Ped m_Suspect2;

        public override bool OnBeforeCalloutDisplayed()
        {
            if (ResponseVLib.WorldZone.GetArea(Game.LocalPlayer.Character.Position) != ResponseVLib.WorldZone.EWorldArea.Los_Santos) return false;

            g_bCustomSpawn = true;

            Vector3 Location = ResponseVLib.Utils.GetRandValue(ResponseVLib.Utils.GetClosestVector3(GTAV.SpawnPoints.s_LosSantosBarFightSpawnPoints, Game.LocalPlayer.Character.Position, 1000.0f));

            g_SpawnPoint = Location;

            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_BARFIGHT)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_BARFIGHT, Enums.EResponse.R_CODE2OR3)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("BarFight: Callout accepted");

            m_Suspect1 = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };
            m_Suspect2 = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                g_Logger.Log("BarFight: Callout accepted");
                Rage.Native.NativeFunction.Natives.SET_PED_MOVEMENT_CLIPSET(m_Suspect1, "move_m@drunk@verydrunk");
                Rage.Native.NativeFunction.Natives.SET_PED_MOVEMENT_CLIPSET(m_Suspect2, "move_m@drunk@verydrunk");

                // Reset
                /*Rage.Native.NativeFunction.Natives.RESET_PED_MOVEMENT_CLIPSET(m_Suspect1, 0.0f);
                Rage.Native.NativeFunction.Natives.RESET_PED_MOVEMENT_CLIPSET(m_Suspect2, 0.0f);*/
            });

            Main.s_GameFibers.Add(fiber);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                m_Suspect1.Tasks.FightAgainst(m_Suspect2);
                m_Suspect2.Tasks.FightAgainst(m_Suspect1);

                if (Functions.IsPedArrested(m_Suspect1) || Functions.IsPedArrested(m_Suspect2) || m_Suspect1.IsDead || m_Suspect2.IsDead)
                {
                    End();
                }
            }
        }

        public override void End()
        {
            m_Suspect1?.Dismiss();
            m_Suspect2?.Dismiss();

            base.End();
        }
    }
}