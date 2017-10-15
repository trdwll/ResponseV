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
        private Vector3 SpawnPoint;
        private Ped ped;
        private Vehicle veh;
        private Blip blip;

        private Model[] vehicleModels = Model.VehicleModels;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(/*Utils.getRandInt(300, 350)*/150f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of " + (Utils.GetRandInt(0, 2) == 1 ? "a" : "a Possible") + " DUI";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.DUI)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            veh = new Vehicle(Utils.GetRandValue(vehicleModels), SpawnPoint);
            veh.IsPersistent = true;

            ped = veh.CreateRandomDriver();
            ped.IsPersistent = true;
            ped.BlockPermanentEvents = true;

            blip = ped.AttachBlip();
            blip.IsFriendly = false;
            
            ped.Tasks.CruiseWithVehicle(Utils.GetRandInt(5, 45));
            //ped.Tasks.PerformDrivingManeuver(VehicleManeuver.SwerveLeft);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 50 && Functions.IsPedArrested(ped))
            {
                End();
            }
        }

        public override void End()
        {
            blip.Delete();
            ped.Dismiss();

            base.End();
        }
    }
}