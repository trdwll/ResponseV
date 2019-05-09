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
    [CalloutInfo("VehicleFire", CalloutProbability.VeryHigh)]
    public class VehicleFire : Callout
    {
        private Vector3 m_SpawnPoint;
        private Vehicle m_Vehicle;
        private Ped m_Victim;
        private Blip m_Blip;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of a Vehicle Fire";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.VEHICLE_FIRE)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), m_SpawnPoint)
            {
                EngineHealth = 350.0f
            };
            m_Victim = m_Vehicle.CreateRandomDriver();
            m_Victim.Tasks.LeaveVehicle(m_Vehicle, LeaveVehicleFlags.LeaveDoorOpen);

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.Yellow);

            LSPDFR.RequestEMS(m_SpawnPoint);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50)
            {
                m_Victim.Kill();
                m_Vehicle.Explode(false);
                m_Blip.IsRouteEnabled = false;
                //if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(m_Victim) == true)
                //{
                //    End();
                //}

                //if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(m_Victim) == false)
                //{
                //    Arrest_Manager.API.Functions.CallCoroner(true);
                //    End();
                //}
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