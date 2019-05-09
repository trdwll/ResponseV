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
        private Vector3 m_SpawnPoint;
        private Blip m_Blip;
        private Enums.Callout m_State;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(300, 1500)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            string type = Utils.GetRandValue("a Shooting", "a Stabbing", "an Armed Car Robbery", "an Armed Robbery", "a Robbery", "an Officer Down", "Multiple Officers Down", 
                "a DUI", "an Aircraft Crash", "a Civilian on Fire", "a Kidnapping", "a Dead Body", "an Overdose");

            CalloutMessage = "Reports of " + type;
            CalloutPosition = m_SpawnPoint;

            // Come up with way to get a safe name from the audio files for above
            // and play the dispatch audio for this call since the immersion isn't there if no audio doesn't play

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_State = Enums.Callout.EnRoute;

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.White);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (m_State == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 20)
            {
                m_State = Enums.Callout.OnScene;

                End();
            }
        }

        public override void End()
        {
            Game.DisplayHelp("Dispatch, call is a prank - resuming patrol.");
            m_Blip.Delete();

            base.End();
        }
    }
}