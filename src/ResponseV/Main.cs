using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSPD_First_Response.Mod.API;
using Rage;

namespace ResponseV
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += this.OnDutyStateChangedEvent;
            
        }

        private void OnDutyStateChangedEvent(bool onDuty)
        {
            if (!onDuty) return;

            // Fed callouts (for only Feds)
            Functions.RegisterCallout(typeof(Callouts.Fed.TerroristPlot));
            Functions.RegisterCallout(typeof(Callouts.Fed.TerroristAttack));
            Functions.RegisterCallout(typeof(Callouts.Fed.Importing));
            Functions.RegisterCallout(typeof(Callouts.Fed.Forgery));
            Functions.RegisterCallout(typeof(Callouts.Fed.IllegalDeal));

            // LSSD callouts (for only LSSD)
            Functions.RegisterCallout(typeof(Callouts.LSSD.PrisonBreak));
            Functions.RegisterCallout(typeof(Callouts.LSSD.PrisonRiot));

            // LSPD calouts (for only LSPD)
            Functions.RegisterCallout(typeof(Callouts.LSPD.GangActivity));

            // Any callouts (for all types of law enforcement)
            Functions.RegisterCallout(typeof(Callouts.Any.BarFight));
            Functions.RegisterCallout(typeof(Callouts.Any.AttemptedSuicide));
            Functions.RegisterCallout(typeof(Callouts.Any.AttemptedMurder));
            Functions.RegisterCallout(typeof(Callouts.Any.AnimalAttack));
            Functions.RegisterCallout(typeof(Callouts.Any.Fight));
            Functions.RegisterCallout(typeof(Callouts.Any.OfficerDown));
            Functions.RegisterCallout(typeof(Callouts.Any.MVA));
            Functions.RegisterCallout(typeof(Callouts.Any.VehicleFire));
            Functions.RegisterCallout(typeof(Callouts.Any.PedHitByVehicle));
            Functions.RegisterCallout(typeof(Callouts.Any.Robbery));
            Functions.RegisterCallout(typeof(Callouts.Any.Pursuit));
            Functions.RegisterCallout(typeof(Callouts.Any.PersonWithWeapon));
            Functions.RegisterCallout(typeof(Callouts.Any.Kidnapping));
            Functions.RegisterCallout(typeof(Callouts.Any.SexOffender));
            Functions.RegisterCallout(typeof(Callouts.Any.SuspiciousItem));
            Functions.RegisterCallout(typeof(Callouts.Any.DeadBody));
            Functions.RegisterCallout(typeof(Callouts.Any.Vandalism));
            Functions.RegisterCallout(typeof(Callouts.Any.ParkingViolation));
            Functions.RegisterCallout(typeof(Callouts.Any.MissingPerson));
            Functions.RegisterCallout(typeof(Callouts.Any.Loitering));

            // Fire Dept
            // maintain a fire scene etc
        }

        public override void Finally() { }
    }
}
