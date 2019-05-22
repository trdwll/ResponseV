using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV.Callouts.Any
{
    internal sealed class AssaultOnOfficer : CalloutBase
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            // Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_ASSAULTONOFFICER, true)}", g_SpawnPoint); // Knife attack
            // Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_ASSAULTONOFFICER, false)}", g_SpawnPoint); // attack

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
        }
    }
}
