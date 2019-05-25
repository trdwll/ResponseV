using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Overdose", CalloutProbability.Medium)]
    internal sealed class Overdose : CalloutBase
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_OVERDOSE)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_OVERDOSE, Enums.EResponse.R_CODE2OR3)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("Overdose: Callout accepted");

            g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360)));

            if (Native.GetSafeCoordForPed(g_SpawnPoint, out Vector3 pos))
            {
                g_Victims.ForEach(v => v.Position = pos);
            }

            g_Victims.ForEach(v =>
            {
                v.Kill();

                if (Main.g_bBetterEMS)
                {
                    BetterEMS.API.EMSFunctions.OverridePedDeathDetails(v, "", "Overdose", Game.GameTime, (float)MathHelper.GetRandomDouble(0.0, 1.0) + .1f);
                }
            });

            GameFiber fiber = GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(2000);
                LSPDFR.RequestEMS(g_SpawnPoint);
            }, "OverdoseRequestEMSFiber");

            Main.g_GameFibers.Add(fiber);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            if (g_bOnScene)
            {
                if (!Utils.m_bCheckingEMS)
                {
                    g_Logger.Log("Overdose: Checking for EMS");
                    Utils.CheckEMSOnScene(g_SpawnPoint, "Overdose");
                }

                if (Utils.m_bEMSOnScene)
                {
                    g_Logger.Log("Overdose: EMS on Scene, end call.");
                    if (g_Victims.Exists(v => v.IsDead))
                    {
                        LSPDFR.RequestCoroner(g_SpawnPoint);
                    }

                    End();
                }
            }
        }
    }
}