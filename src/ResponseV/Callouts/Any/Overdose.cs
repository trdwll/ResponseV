﻿using System;
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
    [CalloutInfo("Overdose", CalloutProbability.Medium)]
    public class Overdose : Callout
    {
        private Vector3 m_SpawnPoint;
        private Ped m_Victim;
        private Blip m_Blip;

        private Model[] m_Models = Model.PedModels;

        public override bool OnBeforeCalloutDisplayed()
        {
            m_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.GetRandInt(500, 700)));

            ShowCalloutAreaBlipBeforeAccepting(m_SpawnPoint, 25f);

            CalloutMessage = $"Reports of " + (Utils.GetRandInt(0, 2) == 1 ? "an" : "a Possible") + " Overdose";
            CalloutPosition = m_SpawnPoint;

            Functions.PlayScannerAudioUsingPosition(
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.WE_HAVE)} " +
                $"{LSPDFR.Radio.GetRandomSound(LSPDFR.Radio.OVERDOSE)} IN_OR_ON_POSITION", m_SpawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            m_Victim = new Ped(Utils.GetRandValue(m_Models), m_SpawnPoint, Utils.GetRandInt(1, 360));

            if (Native.GetSafeCoordForPed(m_SpawnPoint, out Vector3 pos))
            {
                m_Victim.Position = pos;
            }

            m_Blip = new Blip(m_SpawnPoint)
            {
                IsRouteEnabled = true
            };
            m_Blip.EnableRoute(System.Drawing.Color.Orange);

            m_Victim.Kill();

            // BetterEMS.API.EMSFunctions.OverridePedDeathDetails(m_Victim, "", "Overdose", Game.GameTime, (float)Utils.GetRandDouble()+.1f);

            LSPDFR.RequestEMS(m_Victim.Position);

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.Position.DistanceTo(m_SpawnPoint) < 20)
            {
                m_Blip.IsRouteEnabled = false;
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
            m_Blip.Delete();
            m_Victim.Dismiss();

            base.End();
        }
    }
}