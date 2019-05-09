using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Robbery", CalloutProbability.VeryHigh)]
    public class Robbery : Callout
    {
        private Vector3 m_SpawnPoint;
        private Vehicle m_Vehicle;
        private Ped m_Suspect;
        private Blip m_Blip;
        private LHandle m_Pursuit;
        private Enums.Callout m_State;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(600, 700)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of an Armed Robbery";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_ARMED_ROBBERY IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_State = Enums.Callout.EnRoute;

            m_Vehicle = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), m_SpawnPoint);
            m_Suspect = m_Vehicle.CreateRandomDriver();
            m_Vehicle.Driver.Tasks.CruiseWithVehicle(Utils.GetRandInt(40, 80));

            m_Blip = new Blip(m_SpawnPoint);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (m_State == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50)
            {
                m_State = Enums.Callout.OnScene;

                Pursuit();
            }

            if (m_State == Enums.Callout.InPursuit)
            {
                if (!Functions.IsPursuitStillRunning(m_Pursuit))
                {
                    m_State = Enums.Callout.Done;
                    End();
                }
            }
        }

        void Pursuit()
        {
            GameFiber.StartNew(delegate
            {
                m_Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(m_Pursuit, m_Suspect);
                m_Blip.Delete();

                m_State = Enums.Callout.InPursuit;

                LSPDFR.RequestBackup(Game.LocalPlayer.Character.Position, 1, LSPD_First_Response.EBackupResponseType.Pursuit, LSPD_First_Response.EBackupUnitType.LocalUnit);
            });
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Vehicle.Dismiss();

            base.End();
        }
    }
}