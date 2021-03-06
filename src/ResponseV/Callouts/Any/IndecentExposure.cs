﻿using System.Linq;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("IndecentExposure", CalloutProbability.Low)]
    internal sealed class IndecentExposure : CalloutBase
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_INDECENTEXPOSURE)}";

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_INDECENTEXPOSURE, Enums.EResponse.R_CODE2)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("IndecentExposure: Callout accepted");
            Model[] PedModels = { "a_f_m_fatcult_01", "a_m_m_acult_01", "a_m_y_acult_01", "a_m_y_acult_02" };

            g_Suspects.Add(new Ped(ResponseVLib.Utils.GetRandValue(PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360)));
            g_Suspects.ForEach(s => s.Tasks.Wander());

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            // TODO: Fix this to be able to support multiple suspects            
            g_Suspects.ForEach(s => 
            {
                if (Functions.IsPedGettingArrested(s) || Functions.IsPedArrested(s) || s.IsDead)
                {
                    End();
                }
            });
        }
    }
}