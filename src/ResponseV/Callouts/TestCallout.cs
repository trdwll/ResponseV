using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV.Callouts
{
    [CalloutInfo("TestCallout", CalloutProbability.Always)]
    internal sealed class TestCallout : CalloutBase
    {
        private LHandle m_Pursuit;

        public override bool OnBeforeCalloutDisplayed()
        {
            var l = Functions.GetPursuitPeds(m_Pursuit);
            foreach (var e in l)
            {
                if (e.CurrentVehicle.Model.Name == "")
                {
                    // TODO: play random sound such as refueling or going down etc
                }
            }

            return base.OnBeforeCalloutDisplayed();
        }

    }
}
