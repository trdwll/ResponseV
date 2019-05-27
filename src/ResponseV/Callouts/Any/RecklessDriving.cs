using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("DUI", CalloutProbability.Medium)]
    internal sealed class RecklessDriving : CalloutBase
    {
        private Vehicle m_Vehicle;
        private Ped m_Suspect;
        private Blip m_SuspectBlip;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of reckless driver";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} {LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.RECKLESS_DRIVER)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("RecklessDriving: Callout accepted");

            m_Vehicle = new Vehicle(ResponseVLib.Utils.GetRandValue(g_Vehicles), g_SpawnPoint)
            {
                IsPersistent = true
            };

            m_Suspect = m_Vehicle.CreateRandomDriver();
            m_Suspect.IsPersistent = true;
            m_Suspect.BlockPermanentEvents = true;
            m_Suspect.Tasks.CruiseWithVehicle(MathHelper.GetRandomInteger(5, 20));

            m_SuspectBlip = m_Suspect.AttachBlip();

            RecklessDrivingFiber();

            if (Main.s_bTrafficPolicer && ResponseVLib.Utils.GetRandBool())
            {
                // drugs
                Traffic_Policer.API.Functions.SetPedDrugsLevels(m_Suspect, ResponseVLib.Utils.GetRandBool(), ResponseVLib.Utils.GetRandBool());
            }
            
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_Suspect) > 400 && g_bOnScene)
            {
                g_Logger.Log("RecklessDriving: Suspect got away (your distance from the suspect was greater than 400), end call");
                Utils.Notify("Suspect got away.");
                End();
            }

            if (Functions.IsPedArrested(m_Suspect) || m_Suspect.IsDead)
            {
                g_Logger.Log("RecklessDriving: The suspect was arrested or is dead, end call");
                End();
            }
        }

        private void RecklessDrivingFiber()
        {
            g_Logger.Log("RecklessDriving: StartVehicleReckless Fiber");
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                try
                {
                    while (m_Suspect.IsInAnyVehicle(false))
                    {
                        m_Suspect.Tasks.PerformDrivingManeuver(m_Vehicle, VehicleManeuver.SwerveLeft);
                        GameFiber.Sleep(500);
                        m_Suspect.Tasks.PerformDrivingManeuver(m_Vehicle, VehicleManeuver.SwerveRight);
                        GameFiber.Sleep(500);
                        m_Suspect.Tasks.PerformDrivingManeuver(m_Vehicle, VehicleManeuver.SwerveLeft);
                        GameFiber.Sleep(500);
                        m_Suspect.Tasks.CruiseWithVehicle(m_Vehicle, MathHelper.GetRandomInteger(5, 20), VehicleDrivingFlags.Normal);
                        GameFiber.Sleep(5500);
                    }
                }
                catch (System.Exception e)
                {
                    Utils.CrashNotify();
                    g_Logger.Log(e.StackTrace, Logger.ELogLevel.LL_TRACE);
                    End();
                }
            }, "RecklessDrivingFiber");

            Main.s_GameFibers.Add(fiber);
        }

        public override void End()
        {
            g_Logger.Log("RecklessDriving: Callout ended");

            m_Suspect?.Dismiss();
            m_Vehicle?.Dismiss();
            m_SuspectBlip?.Delete();

            base.End();
        }
    }
}
