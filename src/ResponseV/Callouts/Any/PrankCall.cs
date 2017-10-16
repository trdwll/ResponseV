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
    [CalloutInfo("PrankCall", CalloutProbability.VeryHigh)]
    public class PrankCall : Callout
    {
        private Vector3 SpawnPoint;
        private Blip blip;
        private Enums.Callout state;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(300, 1500)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            string type = Utils.GetRandValue("a Shooting", "a Stabbing", "an Armed Car Robbery", "an Armed Robbery", "a Robbery", "an Officer Down", "Multiple Officers Down", 
                "a DUI", "an Aircraft Crash", "a Civilian on Fire", "a Kidnapping", "a Dead Body", "an Overdose");

            CalloutMessage = "Reports of " + type;
            CalloutPosition = SpawnPoint;

            // Come up with way to get a safe name from the audio files for above
            // and play the dispatch audio for this call since the immersion isn't there if no audio doesn't play

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            state = Enums.Callout.EnRoute;

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.White);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (state == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 20)
            {
                state = Enums.Callout.OnScene;

                End();
            }
        }

        public override void End()
        {
            Game.DisplayHelp("Dispatch, call is a prank - resuming patrol.");
            blip.Delete();

            base.End();
        }
    }
}