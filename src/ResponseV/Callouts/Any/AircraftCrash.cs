using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

using LSPD_First_Response.Engine.Scripting;
namespace ResponseV.Callouts.Any
{
    [CalloutInfo("AircraftCrash", CalloutProbability.VeryHigh)]
    public class AircraftCrash : Callout
    {
        private Vector3 m_SpawnPoint;
        private Vehicle m_Aircraft;
        private List<Ped> m_Victims = new List<Ped>();
        private Blip m_Blip;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 50f);

            CalloutMessage = "Reports of an Aircraft Crash";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.AIRCRAFT_CRASH)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            return base.OnCalloutAccepted();
        }

        public override void OnCalloutNotAccepted()
        {
            base.OnCalloutNotAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 50)
            {
                // spawn plane/heli
                // spawn peds and kill
                // decorate peds with blood
            }
        }

        public override void End()
        {

            base.End();
        }
    }
}