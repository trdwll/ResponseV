using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;
using static ResponseV.Enums;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PrankCall", CalloutProbability.Medium)]
    public class PrankCall : RVCallout
    {
        private ECallType m_CallType;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_CallType = RandomEnumValue<ECallType>();
            string type = Utils.GetRandValue("a Shooting", "a Stabbing", "an Armed Car Robbery", "an Armed Robbery", "a Robbery", "an Officer Down", "Multiple Officers Down", 
                "a DUI", "an Aircraft Crash", "a Civilian on Fire", "a Kidnapping", "a Dead Body", "an Overdose");

            CalloutMessage = "Reports of " + LSPDFR.Radio.GetCallTypeFromEnum(m_CallType);
            CalloutPosition = g_SpawnPoint;

            // Come up with way to get a safe name from the audio files for above
            // and play the dispatch audio for this call since the immersion isn't there if no audio doesn't play

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("PrankCall: Callout accepted");

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                Utils.Notify("Dispatch the call was a prank, returning to patrol.");
                End();
            }
        }
    }
}