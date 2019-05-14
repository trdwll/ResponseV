using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;
using static ResponseV.Enums;
using Callout = LSPD_First_Response.Mod.Callouts.Callout;
using ResponseV.GTAV;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PrankCall", CalloutProbability.VeryHigh)]
    public class PrankCall : Callout
    {
        private Vector3 m_SpawnPoint;

        private Blip m_CallBlip;

        private ECallType m_CallType;
        private string m_CallTypeString;

        private bool m_bOnScene;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_CallType = ECallType.CT_DROWNING;//RandomEnumValue<ECallType>();
            m_CallTypeString = LSPDFR.Radio.GetCallTypeFromEnum_PrankCall(m_CallType);

            Vector3 LocalPos = Game.LocalPlayer.Character.Position;

            switch (m_CallType)
            {
            default:
                m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(Configuration.config.Callouts.MinRadius, Configuration.config.Callouts.MaxRadius)));
                Main.MainLogger.Log($"PrankCall: SpawnPoint defaulted.");
                break;
            case ECallType.CT_DROWNING:
                // TODO: Get the nearest one to the player rather than having it spawn 4m away lol
                m_SpawnPoint = LocalPos.GetArea() == Extensions.EWorldArea.Blaine_County ? Utils.GetRandValue(SpawnPoints.BlaineNearWaterSpawnPoints) : Utils.GetRandValue(SpawnPoints.LosSantosNearWaterSpawnPoints);
                Main.MainLogger.Log($"PrankCall: SpawnPoint fixed for drowning.");
                break;
            }

            CalloutMessage = $"(PRANKCALL DEBUG) - Reports of {m_CallTypeString}";
            CalloutPosition = m_SpawnPoint;

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            // Come up with way to get a safe name from the audio files for above
            // and play the dispatch audio for this call since the immersion isn't there if no audio doesn't play

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            Main.MainLogger.Log($"PrankCall: Callout accepted - [{m_CallTypeString}]");

            m_CallBlip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };

            m_CallBlip.EnableRoute(System.Drawing.Color.Blue);
            m_CallBlip.Color = System.Drawing.Color.Blue;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 35 && !m_bOnScene)
            {
                m_bOnScene = true;
                m_CallBlip.IsRouteEnabled = false;
                Main.MainLogger.Log("PrankCall: DistanceTo(m_SpawnPoint) < 35 so hide m_CallBlip");
            }

            if (m_bOnScene)
            {
                Utils.Notify("Dispatch the call was a prank, returning to patrol.");
                End();
            }

            if (Game.LocalPlayer.IsDead)
            {
                Functions.PlayScannerAudio($"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OFFICER_DOWN)} OFFICER_NEEDS_IMMEDIATE_ASSISTANCE");
                End();
                Main.MainLogger.Log("PrankCall: Player is dead so force end call.");
            }
        }

        public override void End()
        {
            m_CallBlip.Delete();

            m_bOnScene = false;

            base.End();
        }
    }
}