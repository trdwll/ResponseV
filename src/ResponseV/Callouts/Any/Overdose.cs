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
    [CalloutInfo("Overdose", CalloutProbability.Medium)]
    public class Overdose : Callout
    {
        private Vector3 SpawnPoint;
        private Ped vic;
        private Blip blip;

        private Model[] models = Model.PedModels;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 700)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = $"Reports of " + (Utils.GetRandInt(0, 2) == 1 ? "an" : "a Possible") + " Overdose";
            CalloutPosition = SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.getRandomSound(LSPDFR.Radio.OVERDOSE)} IN_OR_ON_POSITION", SpawnPoint);

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
            blip.EnableRoute(System.Drawing.Color.Orange);

            vic.Kill();

            BetterEMS.API.EMSFunctions.OverridePedDeathDetails(vic, "", "Overdose", Game.GameTime, (float)new Random().NextDouble()+.1f);

            LSPDFR.RequestEMS(vic.Position);

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
                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(vic) == true)
                {
                    End();
                }

                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(vic) == false)
                {
                    Game.DisplaySubtitle("coroner();");
                    End();
                }
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