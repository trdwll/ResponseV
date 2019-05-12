using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PrankCall", CalloutProbability.VeryHigh)]
    public class PrankCall : RVCallout
    {
        private enum ECallType
        {
            CT_SHOOTING,
            CT_STABBING,
            CT_ARMEDCARROBBERY,
            CT_ARMEDROBBERY,
            CT_ROBBERY,
            CT_OFFICERDOWN,
            CT_MULTIPLEOFFICERDOWN,
            CT_DUI,
            CT_AIRCRAFTCRASH,
            CT_CIVILIANFIRE,
            CT_KIDNAPPING,
            CT_DEADBODY,
            CT_OVERDOSE
        }

        private ECallType m_CallType;

        public override bool OnBeforeCalloutDisplayed()
        {
            string type = Utils.GetRandValue("a Shooting", "a Stabbing", "an Armed Car Robbery", "an Armed Robbery", "a Robbery", "an Officer Down", "Multiple Officers Down", 
                "a DUI", "an Aircraft Crash", "a Civilian on Fire", "a Kidnapping", "a Dead Body", "an Overdose");

            CalloutMessage = "Reports of " + type;
            CalloutPosition = g_SpawnPoint;

            // Come up with way to get a safe name from the audio files for above
            // and play the dispatch audio for this call since the immersion isn't there if no audio doesn't play

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            if (Game.LocalPlayer.Character.Position.DistanceTo(g_SpawnPoint) < 30)
            {
                Utils.Notify("Dispatch the call was a prank, returning to patrol.");
                End();
            }
        }
    }
}