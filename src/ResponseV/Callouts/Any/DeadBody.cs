using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("DeadBody", CalloutProbability.Low)]
    public class DeadBody : Callout
    {
        private Vector3 SpawnPoint;
        private Ped vic;
        private Blip blip;

        private Model[] models = Model.PedModels;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(350, 400)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = $"Reports of a " + (Utils.GetRandInt(0, 2) == 1 ? "Deceased Person" : "Dead Body");
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.DEADBODY)} IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            vic = new Ped(Utils.GetRandValue(models), SpawnPoint, Utils.GetRandInt(1, 360));

            if (Native.GetSafeCoordForPed(SpawnPoint, out Vector3 pos))
            {
                vic.Position = pos;
            }

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.Black);
            
            vic.Kill();

            DamagePack.ApplyDamagePack(vic, 
                Utils.GetRandValue(
                    DamagePack.BigHitByVehicle, DamagePack.SCR_Torture, 
                    DamagePack.Explosion_Large, DamagePack.Burnt_Ped_0,
                    DamagePack.Car_Crash_Light, DamagePack.Car_Crash_Heavy,
                    DamagePack.Fall_Low, DamagePack.Fall,
                    DamagePack.HitByVehicle, DamagePack.BigRunOverByVehicle,
                    DamagePack.RunOverByVehicle
                ), 100, 1);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();
            
            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 20)
            {
                blip.IsRouteEnabled = false;
                Arrest_Manager.API.Functions.CallCoroner(SpawnPoint, true);
                End();
            }
        }

        public override void End()
        {
            blip.Delete();
            vic.Dismiss();

            base.End();
        }
    }
}
