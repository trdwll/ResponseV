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
            CalloutMessage = "Reports of " + (Utils.GetRandBool() ? "a" : "a Possible") + " DUI";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} {LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.DUI)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("DUI: Callout accepted");

            m_Vehicle = new Vehicle(Utils.GetRandValue(g_Vehicles), g_SpawnPoint)
            {
                IsPersistent = true
            };

            m_Suspect = m_Vehicle.CreateRandomDriver();
            m_Suspect.IsPersistent = true;
            m_Suspect.BlockPermanentEvents = true;
            m_Suspect.Tasks.CruiseWithVehicle(Utils.GetRandInt(5, 20));

            m_SuspectBlip = m_Suspect.AttachBlip();

            StartVehicleDUI();

            if (Main.g_bTrafficPolicer)
            {
                if (Utils.GetRandBool())
                {
                    // DUI/drinking
                    Traffic_Policer.API.Functions.SetPedAlcoholLevel(m_Suspect, Traffic_Policer.API.Functions.GetRandomOverTheLimitAlcoholLevel());
                }
                else
                {
                    // drugs
                    Traffic_Policer.API.Functions.SetPedDrugsLevels(m_Suspect, Utils.GetRandBool(), Utils.GetRandBool());

                    if (Utils.GetRandBool())
                    {
                        Traffic_Policer.API.Functions.SetPedAlcoholLevel(m_Suspect, Utils.GetRandBool() ? 
                            Traffic_Policer.API.Functions.GetRandomOverTheLimitAlcoholLevel() : 
                            Traffic_Policer.API.Functions.GetRandomUnderTheLimitAlcoholLevel());
                    }
                }
            }

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_Suspect) > 300 && g_bOnScene)
            {
                g_Logger.Log("DUI: Suspect got away (your distance from the suspect was greater than 300), end call");
                Utils.Notify("Suspect got away.");
                End();
            }

            if (Functions.IsPedArrested(m_Suspect) || m_Suspect.IsDead)
            {
                g_Logger.Log("DUI: The suspect was arrested or is dead, end call");
                End();
            }
        }

        private void StartVehicleDUI()
        {
            g_Logger.Log("DUI: StartVehicleDUI Fiber");
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
            g_Logger.Log("DUI: Callout ended");

            m_Suspect?.Dismiss();
            m_Vehicle?.Dismiss();
            m_SuspectBlip?.Delete();

            base.End();
        }
    }
}