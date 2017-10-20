﻿using System;
using LSPD_First_Response.Mod.API;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
namespace ResponseV
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFR);
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEvent;

            Rage.Game.DisplayNotification($"Response~y~V~w~ ~b~{Configuration.APPVERSION} ~w~loaded successfully");
            
        }

        private void OnDutyStateChangedEvent(bool onDuty)
        {
            if (!onDuty) return;


            // Fed
            //Functions.RegisterCallout(typeof(Callouts.Fed.TerroristPlot));
            //Functions.RegisterCallout(typeof(Callouts.Fed.TerroristAttack));
            //Functions.RegisterCallout(typeof(Callouts.Fed.Importing));
            //Functions.RegisterCallout(typeof(Callouts.Fed.Forgery));
            //Functions.RegisterCallout(typeof(Callouts.Fed.IllegalDeal));

            // LSSD
            //Functions.RegisterCallout(typeof(Callouts.LSSD.PrisonBreak));
            //Functions.RegisterCallout(typeof(Callouts.LSSD.PrisonRiot));

            // LSPD
            //Functions.RegisterCallout(typeof(Callouts.LSPD.GangActivity));

            // Any
            //Functions.RegisterCallout(typeof(Callouts.Any.BarFight));
            //Functions.RegisterCallout(typeof(Callouts.Any.AttemptedSuicide));
            //Functions.RegisterCallout(typeof(Callouts.Any.AttemptedMurder));
            //Functions.RegisterCallout(typeof(Callouts.Any.AnimalAttack));
            //Functions.RegisterCallout(typeof(Callouts.Any.Assault));
            //Functions.RegisterCallout(typeof(Callouts.Any.MVA));
            //Functions.RegisterCallout(typeof(Callouts.Any.PedHitByVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Any.Pursuit));
            //Functions.RegisterCallout(typeof(Callouts.Any.Kidnapping));
            //Functions.RegisterCallout(typeof(Callouts.Any.SexOffender));
            //Functions.RegisterCallout(typeof(Callouts.Any.SuspiciousItem));
            //Functions.RegisterCallout(typeof(Callouts.Any.Vandalism));
            //Functions.RegisterCallout(typeof(Callouts.Any.MissingPerson));
            //Functions.RegisterCallout(typeof(Callouts.Any.Loitering));
            //Functions.RegisterCallout(typeof(Callouts.Any.Littering));
            //Functions.RegisterCallout(typeof(Callouts.Any.DrugBust));

            //Functions.RegisterCallout(typeof(Callouts.Any.Arson));
            //Functions.RegisterCallout(typeof(Callouts.Any.Explosion));
            //Functions.RegisterCallout(typeof(Callouts.Any.DriveBy));
            //Functions.RegisterCallout(typeof(Callouts.Any.StolenVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Any.Theft));
            //Functions.RegisterCallout(typeof(Callouts.Any.PublicIntoxication));
            //Functions.RegisterCallout(typeof(Callouts.Any.Racing));
            //Functions.RegisterCallout(typeof(Callouts.Any.AircraftCrash));

            // Nature
            //Functions.RegisterCallout(typeof(Callouts.Nature.IllegalHunting));
            //Functions.RegisterCallout(typeof(Callouts.Nature.Poaching));
            //Functions.RegisterCallout(typeof(Callouts.Nature.AnimalVsVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Nature.OverKillLimit));
            //Functions.RegisterCallout(typeof(Callouts.Nature.AnimalCruelty));
            //Functions.RegisterCallout(typeof(Callouts.Nature.EndangeredSpecies));

            // In Progress
            // Functions.RegisterCallout(typeof(Callouts.Any.DUI));
            //Functions.RegisterCallout(typeof(Callouts.Any.PrankCall));
            // Functions.RegisterCallout(typeof(Callouts.Any.Robbery));


            // Finished/RFC
            //Functions.RegisterCallout(typeof(Callouts.Any.IndecentExposure));
            //Functions.RegisterCallout(typeof(Callouts.Any.VehicleFire));
            //Functions.RegisterCallout(typeof(Callouts.Any.PersonWithWeapon));
            //Functions.RegisterCallout(typeof(Callouts.Any.ParkingViolation));
            //Functions.RegisterCallout(typeof(Callouts.Any.Speeding));
            //Functions.RegisterCallout(typeof(Callouts.Any.CivOnFire));
            //Functions.RegisterCallout(typeof(Callouts.Any.OfficerDown));
            //Functions.RegisterCallout(typeof(Callouts.Any.Overdose));
            //Functions.RegisterCallout(typeof(Callouts.Any.DeadBody));
        }

        public override void Finally() { }

        public static Assembly LSPDFR(object sender, ResolveEventArgs e)
        {
            foreach (Assembly a in Functions.GetAllUserPlugins())
            {
                if (e.Name.ToLower().Contains(a.GetName().Name.ToLower())) return a;
            }
            return null;
        }
    }
}
