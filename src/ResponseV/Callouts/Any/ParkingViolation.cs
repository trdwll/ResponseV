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
        private Vector3 SpawnPoint;
        private Vehicle veh;
        private Blip blip;
        private Enums.Callout state;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)).Around(5f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of an Illegally Parked Vehicle";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.PARKING)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            state = Enums.Callout.EnRoute;

            veh = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), SpawnPoint);
            
            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.White);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (state == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 20)
            {
                state = Enums.Callout.OnScene;
                blip.IsRouteEnabled = false;
                End();
            }
        }

        public override void End()
        {
            blip.Delete();
            veh.Dismiss();

            base.End();
        }
    }
}