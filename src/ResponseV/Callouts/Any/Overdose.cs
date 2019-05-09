using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("Overdose", CalloutProbability.Medium)]
    public class Overdose : RVCallout
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = $"Reports of " + (Utils.GetRandBool() ? "an" : "a Possible") + " Overdose";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OVERDOSE)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Logger.Log("Accepted Overdose Callout", Logger.ELogLevel.LL_INFO);

            m_Victims.Add(new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360)));

            if (Native.GetSafeCoordForPed(m_SpawnPoint, out Vector3 pos))
            {
                m_Victims.ForEach(v => v.Position = pos);
            }

            m_Victims.ForEach(v => v.Kill());

            // BetterEMS.API.EMSFunctions.OverridePedDeathDetails(m_Victim, "", "Overdose", Game.GameTime, (float)Utils.GetRandDouble()+.1f);

            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(5000);
                LSPDFR.RequestEMS(m_SpawnPoint);
            });

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 20)
            {
                End();
                //if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(m_Victim) == true)
                //{
                //    End();
                //}

                //if (BetterEMS.API.EMSFunctions.DidEMSRevivePed(m_Victim) == false)
                //{
                //    Arrest_Manager.API.Functions.CallCoroner(m_SpawnPoint, true);
                //    End();
                //}
            }
        }

        public override void End()
        {
            base.End();
        }
    }
}