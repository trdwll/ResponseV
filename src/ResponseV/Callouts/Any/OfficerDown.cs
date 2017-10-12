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
    [CalloutInfo("OfficerDown", CalloutProbability.VeryHigh)]
    public class OfficerDown : Callout
    {
        private Vector3 SpawnPoint;
        private Ped ped;
        private Blip blip;

        private Model[] models = { "s_m_y_sheriff_01", "s_f_y_sheriff_01", "s_f_y_cop_01", "s_m_y_cop_01", "s_m_y_hwaycop_01" };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(300, 350)));

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
            ped = new Ped(Utils.GetRandValue(models), SpawnPoint, Utils.GetRandInt(1, 360));
            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.Blue);

            ped.Kill();

            LSPDFR.RequestEMS(ped.Position);

            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            // call auto ends when you get to < 20 so figure a way to not end until ems gets on scene

            if (Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 20)
            {
                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(ped) == true)
                {
                    End();
                }

                if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(ped) == false)
                {
                    Game.DisplaySubtitle("coroner();");
                    End();
                }
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