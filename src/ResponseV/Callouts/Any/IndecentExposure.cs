using System.Linq;
using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System.Collections.Generic;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("IndecentExposure", CalloutProbability.Low)]
    public class IndecentExposure : Callout
    {
        private Vector3 m_SpawnPoint;
        private Ped m_Suspect;
        private Blip m_Blip;

        private Enums.Callout m_State;

        private Model[] m_PedModels = { "a_f_m_fatcult_01", "a_m_m_acult_01", "a_m_y_acult_01", "a_m_y_acult_02" };

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(400, 550)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = "Reports of Indecent Exposure";
            CalloutPosition = m_SpawnPoint;
            
            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_INDECENT_EXPOSURE IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_State = Enums.Callout.EnRoute;

            m_Suspect = new Ped(Utils.GetRandValue(m_PedModels), m_SpawnPoint, Utils.GetRandInt(1, 360));
            m_Suspect.Tasks.Wander();

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.Yellow);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (m_State == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 30)
            {
                m_State = Enums.Callout.OnScene;

                m_Blip.IsRouteEnabled = false;
                m_Blip.Delete();
            }

            if (m_State == Enums.Callout.OnScene && (Functions.IsPedGettingArrested(m_Suspect) || Functions.IsPedArrested(m_Suspect) || m_Suspect.IsDead))
            {
                m_State = Enums.Callout.Done;
                End();
            }
        }

        public override void End()
        {
            m_Blip.Delete();
            m_Suspect.Dismiss();

            base.End();
        }
    }
}