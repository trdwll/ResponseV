﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Fed
{
    // The plot is where you respond to (usually code 2) and search the are for clues to where the attack will be
    // and detain anyone that's at the location, it's usually best to have other agents back you up
    [CalloutInfo("TerroristPlot", CalloutProbability.VeryHigh)]
    internal sealed class TerroristPlot : Callout
    {
        private Vector3 g_SpawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            g_SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(MathHelper.GetRandomInteger(400, 600)));

            ShowCalloutAreaBlipBeforeAccepting(g_SpawnPoint, 30f);

            CalloutMessage = "Terrorist Plot"; // Add - _<type>_
            CalloutPosition = g_SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }
    }
}
