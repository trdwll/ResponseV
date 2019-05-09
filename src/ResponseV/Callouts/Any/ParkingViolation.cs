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
    [CalloutInfo("ParkingViolation", CalloutProbability.VeryLow)]
    public class ParkingViolation : Callout
    {
        private Vector3 m_SpawnPoint;
        private Vehicle m_Vehicle;
        private Blip m_Blip;
        private Enums.Callout m_State;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)).Around(5f));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of an Illegally Parked Vehicle";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.PARKING)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_State = Enums.Callout.EnRoute;

            m_Vehicle = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), m_SpawnPoint);
            
            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.White);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (m_State == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 20)
            {
                m_State = Enums.Callout.OnScene;
                m_Blip.IsRouteEnabled = false;
                End();
            }
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Vehicle.Dismiss();

            base.End();
        }
    }
}