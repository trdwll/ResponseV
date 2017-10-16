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
        private Vector3 SpawnPoint;
        private Ped suspect;
        private Blip blip;

        private Enums.Callout state;

        private Model[] pedModels = { "a_f_m_fatcult_01", "a_m_m_acult_01", "a_m_y_acult_01", "a_m_y_acult_02" };

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(400, 550)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 25f);

            CalloutMessage = "Reports of Indecent Exposure";
            CalloutPosition = SpawnPoint;
            
            Functions.PlayScannerAudioUsingPosition($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} CRIME_INDECENT_EXPOSURE IN_OR_ON_POSITION", SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            state = Enums.Callout.EnRoute;

            suspect = new Ped(Utils.GetRandValue(pedModels), SpawnPoint, Utils.GetRandInt(1, 360));
            suspect.Tasks.Wander();

            blip = new Blip(SpawnPoint)
            {
                IsRouteEnabled = true
            };
            blip.EnableRoute(System.Drawing.Color.Yellow);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (state == Enums.Callout.EnRoute && Game.LocalPlayer.Character.Position.DistanceTo(SpawnPoint) < 30)
            {
                state = Enums.Callout.OnScene;

                blip.IsRouteEnabled = false;
                blip.Delete();
            }

            if (state == Enums.Callout.OnScene && (Functions.IsPedGettingArrested(suspect) || Functions.IsPedArrested(suspect) || suspect.IsDead))
            {
                state = Enums.Callout.Done;
                End();
            }
        }

        public override void End()
        {
            blip.Delete();
            suspect.Dismiss();

            base.End();
        }
    }
}