using System.Linq;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("IndecentExposure", CalloutProbability.Low)]
    public class IndecentExposure : RVCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of Indecent Exposure";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_INDECENT_EXPOSURE IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Model[] PedModels = { "a_f_m_fatcult_01", "a_m_m_acult_01", "a_m_y_acult_01", "a_m_y_acult_02" };

            m_Suspects.Add(new Ped(Utils.GetRandValue(PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360)));
            m_Suspects.ForEach(s => s.Tasks.Wander());

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            // TODO: Fix this to be able to support multiple suspects            
            m_Suspects.ForEach(s => 
            {
                if (Functions.IsPedGettingArrested(s) || Functions.IsPedArrested(s) || s.IsDead)
                {
                    End();
                }
            });
        }
    }
}