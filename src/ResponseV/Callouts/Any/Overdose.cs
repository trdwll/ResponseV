using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;
using System.Linq;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Overdose", CalloutProbability.Medium)]
    public class Overdose : RVCallout
    {
        private bool m_bPedIsDead;
        private bool m_bEMSOnScene;
        private bool m_bCheckEMS;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of " + (Utils.GetRandBool() ? "an" : "a Possible") + " Overdose";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OVERDOSE)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("Accepted Overdose Callout", Logger.ELogLevel.LL_INFO);

            g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360)));

            if (Native.GetSafeCoordForPed(g_SpawnPoint, out Vector3 pos))
            {
                g_Victims.ForEach(v => v.Position = pos);
            }

            g_Victims.ForEach(v =>
            {
                v.Kill();

                if (Main.g_bBetterEMS)
                {
                    BetterEMS.API.EMSFunctions.OverridePedDeathDetails(v, "", "Overdose", Game.GameTime, (float)Utils.GetRandDouble()+.1f);
                }
            });

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(5000);
                LSPDFR.RequestEMS(g_SpawnPoint);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            if (g_bOnScene)
            {
                if (!m_bCheckEMS)
                {
                    CheckEMS();
                }

                if (m_bEMSOnScene)
                {
                    m_bPedIsDead = g_Victims.Exists(v => v.IsDead);

                    if (m_bPedIsDead)
                    {
                        LSPDFR.RequestCoroner(g_SpawnPoint);
                    }

                    m_bPedIsDead = false;
                    End();
                }
            }
        }

        void CheckEMS()
        {
            m_bCheckEMS = true;
            GameFiber.StartNew(delegate
            {
                while (!m_bEMSOnScene)
                {
                    GameFiber.Yield();
                    List<Vehicle> vehicles = Game.LocalPlayer.Character.GetNearbyVehicles(16).ToList();

                    vehicles.ForEach(v =>
                    {
                        if (v.Model.Name == "AMBULANCE")
                        {
                            m_bEMSOnScene = true;
                        }
                    });

                    GameFiber.Sleep(10000);
                    vehicles.Clear();
                    g_Logger.Log("Overdose: Checking for nearby EMS");
                }

            }, "CheckEMSFiber");
        }
    }
}