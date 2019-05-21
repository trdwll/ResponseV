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
    /*
137.45, -3200.54, 5.22 = call (30f)
148.85, -3213.21, 5.60, 214.66 = vapid speedo (2)
136.19, -3214.54, 6.09, 180.02 = mule(3) - left (2)
131.14, -3214.48, 6.09, 180.02 = mule(3) - center (1)
124.87, -3214.15, 6.16, 153.60 = mule(3) - right (1)
135.70, -3195.42, 5.77, 119.50 = benefactor xls (armored) (4)
125.76, -3191.93, 6.14, 190.33 = vapid contender (1)
138.93, -3190.64, 5.77, 251.37 = gallivanter baller lwb (armored) (2)
124.34, -3199.89, 5.27, 270.30 = lampadati tropos rallye (2) 

136.82, -3092.64, 5.26 = call (30f)
134.25, -3105.65, 5.26, 182.53 = vapid speedo (1)
130.12, -3105.65, 5.27, 182.53 = vapid speedo (1)
148.47, -3081.08, 5.26, 57.61 = vapid contender (1)
132.85, -3083.11, 5.26, 89.70 = granger (4)

== airport == 
-971.35, -3000.64, 14.59 = call (60f)
-971.35, -3000.64, 14.59, 60 = buckingham nimbus (1)
-981.71, -3010.84, 14.22, 280 = guardian (2)
-989.33, -3004.92, 14.22, 240 = granger (4)
-984.45, -2992.74, 13.33, 291.62 = granger (4)
-967.07, -2966.57, 14.22, 330 = mule(3) - left (2)
-962.24, -2966.57, 14.22, 330 = mule(3) - right (2)
-968.69, -3032.28, 14.22, 60 = mule(3)  (2)
-970.72, -2984.27, 13.92, 263 = vapid benson (2)
sports drift tampa
    */
    // Importing of illegal drugs, weapons, humans, etc - you may want to respond with a good amount of agents on these to
    // avoid anyone from getting away
    [CalloutInfo("Importing", CalloutProbability.VeryHigh)]
    internal sealed class Importing : Callout
    {
        private Vector3 g_SpawnPoint;

        public override bool OnBeforeCalloutDisplayed()
        {
            // SpawnPoint needs to be fixed locations like the ports and airports
            Vector3[] points = new Vector3[] {
                new Vector3(137.45f, -3200.54f, 5.22f), // Port 1
                new Vector3(136.82f, -3092.64f, 5.26f), // Port 2
                new Vector3(-971.35f, -3000.64f, 14.59f) // Airport 1
            };

            g_SpawnPoint = points[new Random().Next(0, points.Length)];

            ShowCalloutAreaBlipBeforeAccepting(g_SpawnPoint, 40f);

            var types = Enum.GetValues(typeof(EType));

            CalloutMessage = $"Reports of illegal importing of {(EType)types.GetValue(new Random().Next(0, types.Length))}";
            CalloutPosition = g_SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }

        private enum EType
        {
            Drugs,
            Weapons,
            Humans
        }
    }
}
