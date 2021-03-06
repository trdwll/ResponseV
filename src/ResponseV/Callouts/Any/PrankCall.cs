﻿using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;
using static ResponseV.Enums;
using Callout = LSPD_First_Response.Mod.Callouts.Callout;
using ResponseV.GTAV;

using World = Rage.World;
using Ped = Rage.Ped;
using ResponseVLib;
using WorldZone = ResponseVLib.WorldZone;

namespace ResponseV.Callouts.Any
{
    [CalloutInfo("PrankCall", CalloutProbability.Medium)]
    internal sealed class PrankCall : Callout
    {
        private Vector3 m_SpawnPoint;

        private Blip m_CallBlip;

        private ECallType m_CallType;
        private string m_CallTypeString;

        private bool m_bOnScene;

        private Ped m_Suspect;
        private Blip m_SuspectBlip;
        private bool m_bCreateSuspect = MathHelper.GetRandomInteger(1, 3) == 1;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_CallType = RandomEnumValue<ECallType>();
            m_CallTypeString = LSPDFR.Radio.GetCallStringFromEnum(m_CallType);

            Vector3 LocalPos = Game.LocalPlayer.Character.Position;
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(MathHelper.GetRandomInteger(ResponseVLib.Configuration.config.Callouts.MinRadius, ResponseVLib.Configuration.config.Callouts.MaxRadius)));

            string CallAudio = $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} ";

            switch (m_CallType)
            {
            default:
                Main.MainLogger.Log($"PrankCall: SpawnPoint defaulted.");
                break;
            case ECallType.CT_AIRCRAFTCRASH:
                CallAudio += $"";
                break;
            case ECallType.CT_SPEEDINGVEHICLE:
                CallAudio += $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.SPEEDING)} {LSPDFR.Radio.GetSpeedSound((uint)MathHelper.GetRandomInteger(40, 120))}";
                break;
            case ECallType.CT_DROWNING:
                // TODO: Get the nearest one to the player rather than having it spawn 4m away lol
                m_SpawnPoint = WorldZone.GetArea(LocalPos) == WorldZone.EWorldArea.Blaine_County ? ResponseVLib.Utils.GetRandValue(SpawnPoints.BlaineDrowningSpawnPoints) : ResponseVLib.Utils.GetRandValue(SpawnPoints.LosSantosDrowningSpawnPoints);
                Main.MainLogger.Log($"PrankCall: SpawnPoint fixed for drowning.");
                break;
            }

            CalloutMessage = $"(PRANKCALL DEBUG) - Reports of {m_CallTypeString}";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(CallAudio, m_SpawnPoint);

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

            if (m_bCreateSuspect)
            {
                m_Suspect = new Ped(ResponseVLib.Utils.GetRandValue(Model.PedModels), m_SpawnPoint, 0f);
                Main.MainLogger.Log("PrankCall: Spawned suspect");
            }

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
                Utils.NotifyPlayerTo("Dispatch", "The call was a prank, returning to patrol.");

                if (m_bCreateSuspect)
                {
                    m_SuspectBlip = m_Suspect.AttachBlip();
                    m_SuspectBlip.Color = System.Drawing.Color.White;
                    m_SuspectBlip.Scale = 0.5f;

                    Utils.NotifyDispatchTo("Player", $"Be advised we've traced the caller and they should be nearby.");
                }
                else
                {
                    End();
                }
            }

            if (m_bCreateSuspect && Functions.IsPedArrested(m_Suspect) || Functions.IsPedGettingArrested(m_Suspect))
            {
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
            Main.MainLogger.Log("PrankCall: Call ended");

            m_CallBlip.Delete();
            m_SuspectBlip?.Delete();
            m_Suspect?.Dismiss();

            m_bOnScene = false;

            base.End();
        }
    }
}