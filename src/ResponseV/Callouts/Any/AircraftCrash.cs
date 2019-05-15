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
    internal class AircraftCrash : CalloutBase
    {
        private Vehicle m_Vehicle;

        private Model[] m_AirplaneModels = { };
        private Model[] m_HelicopterModels = { };

        private bool m_bIsAirplane = Utils.GetRandBool();

        public override bool OnBeforeCalloutDisplayed()
        {
            CalloutMessage = "Reports of an " + (m_bIsAirplane ? Utils.GetRandValue("Aircraft", "Airplane") : "Helicopter") + " Crash";
            CalloutPosition = g_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " + 
                (m_bIsAirplane ? 
                "{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.AIRCRAFT_CRASH)}" : 
                "{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.HELICOPTER_CRASH)}") + 
                " IN_OR_ON_POSITION", g_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Vehicle = new Vehicle((m_bIsAirplane ? Utils.GetRandValue(m_AirplaneModels) : Utils.GetRandValue(m_HelicopterModels)), g_SpawnPoint);

            End();

            //GameFiber.StartNew(delegate
            //{
            //    GameFiber.Sleep(10000);

            //    LSPDFR.RequestBackup(g_SpawnPoint, 2);

            //    GameFiber.Sleep(1000);
            //    LSPDFR.RequestEMS(g_SpawnPoint);

            //    GameFiber.Sleep(1000);
            //    LSPDFR.RequestFire(g_SpawnPoint);
                
            //});

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (g_bOnScene)
            {
                // spawn plane/heli
                // spawn peds and kill
                // decorate peds with blood
            }
        }

        void SceneCreation()
        {

        }

        public override void End()
        {
            m_Vehicle.Dismiss();

            base.End();
        }
    }
}