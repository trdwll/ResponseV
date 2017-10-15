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
        private Vector3 SpawnPoint;
        private Vehicle veh;
        private Ped vic;
        private Blip blip;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 600)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of a Vehicle Fire";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.VEHICLE_FIRE)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            veh = new Vehicle(Utils.GetRandValue(Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle).ToArray()), SpawnPoint)
            {
                EngineHealth = 350.0f,
                IsOnFire = true,
                IsFireProof = true
            };
            vic = veh.CreateRandomDriver();
            vic.Kill();

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.Yellow);

            LSPDFR.RequestEMS(SpawnPoint);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 20)
            {
                veh.IsFireProof = false;
                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(vic) == true)
                {
                    End();
                }

                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(vic) == false)
                {
                    Game.DisplaySubtitle("Request a coroner");
                    End();
                }
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