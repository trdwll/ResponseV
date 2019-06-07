using System.Linq;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("SpeedingVehicle", CalloutProbability.Medium)]
    internal sealed class SpeedingVehicle : CalloutBase
    {
        private Vehicle m_Vehicle;
        private LHandle m_Pursuit;

        private uint m_Speed;

        private bool m_bIsPursuit = ResponseVLib.Utils.GetRandBool();

        private Ped m_Driver;

        private Blip m_SpeedingVehicleBlip;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_Speed = (uint)MathHelper.GetRandomInteger(40, 120);

            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_SPEEDINGVEHICLE)}";

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_SPEEDINGVEHICLE, Enums.EResponse.R_CODE2OR3, m_Speed)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("SpeedingVehicle: Callout accepted");

            m_Vehicle = new Vehicle(ResponseVLib.Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), g_SpawnPoint);
            m_Driver = m_Vehicle.CreateRandomDriver();
            g_Suspects.Add(m_Driver);

            m_SpeedingVehicleBlip = m_Vehicle.AttachBlip();
            m_SpeedingVehicleBlip.Color = System.Drawing.Color.Green;
            m_SpeedingVehicleBlip.Scale = 0.5f;

            m_Vehicle.AnnounceVehicleDetails();

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 150 && !g_bOnScene)
            {
                g_bOnScene = true;
                g_CallBlip.IsRouteEnabled = false;

                m_Vehicle.Driver.KeepTasks = true;
                m_Vehicle.Driver.Tasks.CruiseWithVehicle(m_Speed);
            }

            if (g_bOnScene && !g_bIsPursuit)
            {
                Pursuit();
            }

            if (!g_bIsPursuit && Functions.IsPlayerPerformingPullover() && Functions.GetPulloverSuspect(Functions.GetCurrentPullover()) == m_Driver)
            {
                g_Logger.Log("SpeedingVehicle: Pulling over the speeding vehicle");
                End();
            }

            if (g_bOnScene && g_bIsPursuit && !Functions.IsPursuitStillRunning(m_Pursuit))
            {
                g_Logger.Log("SpeedingVehicle: Pursuit has ended");
                End();
            }
        }

        void Pursuit()
        {
            g_Logger.Log("SpeedingVehicle: Started pursuit");
            g_bIsPursuit = true;
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                GameFiber.Yield();

                m_Pursuit = Functions.CreatePursuit();

                //g_Suspects.ForEach(s =>
                //{
                //    Functions.AddPedToPursuit(m_Pursuit, s);
                //});

                if (m_Driver != null)
                {
                    Functions.AddPedToPursuit(m_Pursuit, m_Driver);
                    g_Logger.Log("SpeedingVehicle: Added ped to pursuit");
                }

                Functions.SetPursuitIsActiveForPlayer(m_Pursuit, true);
                Functions.SetPursuitCopsCanJoin(m_Pursuit, true);

                GameFiber.Sleep(10000);
                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 2, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            }, "SpeedingVehicleFiber");

            Main.s_GameFibers.Add(fiber);
        }

        public override void End()
        {
            m_Vehicle?.Dismiss();
            m_SpeedingVehicleBlip?.Delete();

            g_Logger.Log("SpeedingVehicle: End()");
            base.End();
        }
    }
}