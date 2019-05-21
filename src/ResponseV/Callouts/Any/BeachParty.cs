using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("BeachParty", CalloutProbability.VeryHigh)]
    internal sealed class BeachParty : CalloutBase
    {
        public override bool OnBeforeCalloutDisplayed()
        {
            g_bCustomSpawn = true;

            // TODO: if in Blaine choose those spawnpoints
            g_SpawnPoint = Utils.GetRandValue(GTAV.SpawnPoints.m_LosSantosBeachPartySpawnPoints);

            CalloutMessage = $"Reports of a beach party";
            CalloutPosition = g_SpawnPoint;

            // Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            g_Logger.Log("BeachParty: Callout accepted");

            //g_Victims.Add(new Ped(Utils.GetRandValue(g_PedModels), g_SpawnPoint, Utils.GetRandInt(1, 360)));

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                End();
            }
        }
    }
}
