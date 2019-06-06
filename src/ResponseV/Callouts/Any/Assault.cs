using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Assault", CalloutProbability.Medium)]
    internal sealed class Assault : CalloutBase
    {
        private Ped m_Suspect1;
        private Ped m_Suspect2;

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of {LSPDFR.Radio.GetCallStringFromEnum(Enums.ECallType.CT_ASSAULT)}";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetCalloutAudio(Enums.ECallType.CT_ASSAULT, Enums.EResponse.R_CODE2OR3)}", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("Assault: Callout accepted");

            m_Suspect1 = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };
            m_Suspect2 = new Ped(ResponseVLib.Utils.GetRandValue(g_PedModels), g_SpawnPoint, MathHelper.GetRandomInteger(1, 360))
            {
                IsPersistent = true
            };

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                // TODO: Randomly create a pursuit with m_Suspect1
                m_Suspect1.Tasks.FightAgainst(m_Suspect2);
                
                if (ResponseVLib.Utils.GetRandBool())
                {
                    m_Suspect2.Tasks.FightAgainst(m_Suspect1);
                }

                if (Functions.IsPedArrested(m_Suspect1) || Functions.IsPedArrested(m_Suspect2) || m_Suspect1.IsDead || m_Suspect2.IsDead)
                {
                    End();
                }
            }
        }

        public override void End()
        {
            m_Suspect1?.Dismiss();
            m_Suspect2?.Dismiss();

            base.End();
        }
    }
}