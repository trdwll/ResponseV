using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using Rage.Native;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("DUI", CalloutProbability.VeryHigh)]
    public class DUI : Callout
    {
        private Vector3 m_SpawnPoint;
        private Ped m_Ped;
        private Vehicle m_Vehicle;
        private Blip m_Blip;

        private Model[] m_VehicleModels = Model.VehicleModels;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*Utils.getRandInt(300, 350)*/150f));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of " + (Utils.GetRandInt(0, 2) == 1 ? "a" : "a Possible") + " DUI";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.DUI)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle(Utils.GetRandValue(m_VehicleModels), m_SpawnPoint);
            m_Vehicle.IsPersistent = true;

            m_Ped = m_Vehicle.CreateRandomDriver();
            m_Ped.IsPersistent = true;
            m_Ped.BlockPermanentEvents = true;

            m_Blip = m_Ped.AttachBlip();
            m_Blip.IsFriendly = false;
            
            m_Ped.Tasks.CruiseWithVehicle(Utils.GetRandInt(5, 45));
            //m_Ped.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveLeft);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50 && Functions.IsPedArrested(m_Ped))
            {
                End();
            }
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Ped.Dismiss();

            base.End();
        }
    }
}