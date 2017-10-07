using System;
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
    // Importing of illegal drugs, weapons, humans, etc - you may want to respond with a good amount of agents on these to
    // avoid anyone from getting away
    [CalloutInfo("Importing", CalloutProbability.VeryHigh)]
    public class Importing : Callout
    {
        private Vector3 SpawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            // SpawnPoint needs to be fixed locations like the ports and airports
            Vector3[] points = new Vector3[] {
                new Vector3(137.45f, -3200.54f, 5.22f), // Port 1
                new Vector3(136.82f, -3092.64f, 5.26f), // Port 2
                new Vector3(-971.35f, -3000.64f, 14.59f) // Airport 1
            };
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(Utils.getRandInt(400, 600)));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 40f);

            CalloutMessage = "Reports of illegal importing of cannabis"; // Add _<type>_
            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }
    }
}
