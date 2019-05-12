using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("DUI", CalloutProbability.Medium)]
    public class DUI : RVCallout
    {
        private Vehicle m_Vehicle;
        private Ped m_Suspect;
        private Blip m_SuspectBlip;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of " + (Utils.GetRandInt(0, 2) == 1 ? "a" : "a Possible") + " DUI";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.DUI)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint);
            m_Vehicle.IsPersistent = true;

            m_Suspect = m_Vehicle.CreateRandomDriver();
            m_Suspect.IsPersistent = true;
            m_Suspect.BlockPermanentEvents = true;
            m_Suspect.Tasks.CruiseWithVehicle(Utils.GetRandInt(5, 20));

            m_SuspectBlip = m_Suspect.AttachBlip();

            StartVehicleDUI();

            // TODO: If Traffic Policer is installed? then add DUI 
            // Breathalyzer.SetPedAlcoholLevels(m_Suspect, Breathalyzer.GetRandomOverTheLimitAlcoholLevel());

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            // TODO: possible fleeing/pursuit

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_Suspect) > 300 && g_bOnScene)
            {
                Utils.Notify("Suspect got away.");
                End();
            }

            if (Functions.IsPedArrested(m_Suspect) || m_Suspect.IsDead)
            {
                End();
            }
        }

        private void StartVehicleDUI()
        {
            GameFiber.StartNew(delegate
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
                        m_Suspect.Tasks.CruiseWithVehicle(m_Vehicle, Utils.GetRandInt(5, 20), VehicleDrivingFlags.Normal);
                        GameFiber.Sleep(5500);
                    }
                }
                catch (System.Exception e)
                {
                    Utils.CrashNotify();
                    g_Logger.Log(e.Message, Logger.ELogLevel.LL_TRACE);
                    End();
                }
            });
        }

        public override void End()
        {
            m_Vehicle.Dismiss();
            m_SuspectBlip.Delete();

            base.End();
        }
    }
}