using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.Callouts.Fed
{
    // The attack usually happens after a plot is discovered, but can happen without a plot
    [CalloutInfo("TerroristAttack", CalloutProbability.VeryHigh)]
    internal sealed class TerroristAttack : Callout
    {
    }
}
