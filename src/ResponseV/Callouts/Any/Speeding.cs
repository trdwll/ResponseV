using System.Linq;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Speeding", CalloutProbability.VeryHigh)]
    public class Speeding : RVCallout
    {
        private Vehicle m_Vehicle;
        private LHandle m_Pursuit;

        private bool m_bIsPursuit = Utils.GetRandBool();
        private bool m_bOnScene;

        private Ped m_Driver;

        private Blip m_SpeedingVehicleBlip;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of a Speeding Vehicle";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} "+
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.SPEEDING)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), m_SpawnPoint);
            m_Driver = m_Vehicle.CreateRandomDriver();
            m_Suspects.Add(m_Driver);

            m_SpeedingVehicleBlip = m_Vehicle.AttachBlip();
            m_SpeedingVehicleBlip.Color = System.Drawing.Color.Green;
            m_SpeedingVehicleBlip.Scale = 0.5f;

            // TODO: Notify a description, random will display license plate (partials), colors, etc

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50 && !m_bOnScene)
            {
                m_Vehicle.Driver.KeepTasks = true;
                m_Vehicle.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(50, 80));
            }

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 30)
            {
                m_bOnScene = true;

                if (m_bIsPursuit)
                {
                    Pursuit();
                }
            }

            if (!m_bIsPursuit && Functions.IsPlayerPerformingPullover() && Functions.GetPulloverSuspect(Functions.GetCurrentPullover()) == m_Driver)
            {
                m_Logger.Log("SpeedingVehicle: Pulling over the speeding vehicle");
                End();
            }

            if (m_bOnScene && m_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                m_Logger.Log("SpeedingVehicle: Pursuit has ended");
                End();
            }
        }

        void Pursuit()
        {
            m_Logger.Log("SpeedingVehicle: Started pursuit");
            GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();

                m_Suspects.ForEach(s =>
                {
                    Functions.AddPedToPursuit(m_Pursuit, s);
                });

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);

                GameFiber.Sleep(10000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            });
        }

        public override void End()
        {
            m_Vehicle.Dismiss();
            m_SpeedingVehicleBlip.Delete();

            m_Logger.Log("SpeedingVehicle: End()");
            base.End();
        }
    }
}