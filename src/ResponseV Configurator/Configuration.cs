using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV_Configurator
{
    public class Configuration
    {
        //public class Cfg
        //{
        //    public class RootObject
        //    {
        //        public Roleplay Roleplay = new Roleplay();
        //        public Ambient Ambient = new Ambient();
        //        public Callouts Callouts = new Callouts();
        //        public Misc Misc = new Misc();
        //    }

        //    public class Roleplay
        //    {
        //        public bool Realism { get; set; }
        //        public string OfficerName { get; set; } = "Ofc. Johnson";
        //    }

        //    public class Ambient
        //    {
        //        public bool Call { get; set; }
        //        public bool Patrol { get; set; }
        //        public bool TrafficStop { get; set; }
        //        public bool PedestrianCrimes { get; set; }
        //    }

        //    public class Callouts
        //    {
        //        public int MinRadius { get; set; } = 500;
        //        public int MaxRadius { get; set; } = 1000;

        //        public Dictionary<string, bool> Features { get; set; } = new Dictionary<string, bool>()
        //        {
        //            {"AircraftCrash", true},
        //            {"AnimalAttack", true},
        //            {"AnimalVsVehicle", true},
        //            {"Assault", true},
        //            {"AttemptedMurder", true},
        //            {"AttemptedSuicide", true},
        //            {"BarFight", true},
        //            {"CivOnFire", true},
        //            {"DeadBody", true},
        //            {"DrugBust", true},
        //            {"DUI", true},
        //            {"Forgery", false},
        //            {"GangActivity", true},
        //            {"IllegalDeal", false},
        //            {"IllegalHunting", true},
        //            {"Importing", false},
        //            {"IndecentExposure", true},
        //            {"Kidnapping", true},
        //            {"Littering", true},
        //            {"Loitering", true},
        //            {"MissingPerson", true},
        //            {"MVA", true},
        //            {"OfficerDown", true},
        //            {"Overdose", true},
        //            {"OverKillLimit", true},
        //            {"ParkingViolation", true},
        //            {"PedHitByVehicle", true},
        //            {"PersonWithWeapon", true},
        //            {"Poaching", true},
        //            {"PrankCall", true},
        //            {"PrisonBreak", true},
        //            {"PrisonRiot", true},
        //            {"Pursuit", true},
        //            {"Robbery", true},
        //            {"SexOffender", true},
        //            {"Speeding", true},
        //            {"SuspiciousItem", true},
        //            {"TerroristAttack", false},
        //            {"TerroristPlot", false},
        //            {"Vandalism", true},
        //            {"VehicleFire", true}
        //        };
        //    }

        //    public class Misc
        //    {
        //        public bool TrafficBackup { get; set; }
        //    }
        //}

        public class Cfg
        {
            public struct Callout
            {
                public bool Enabled;
                public uint MinRadius; // TODO: Add this
                public uint MaxRadius; // TODO: Add this
            }

            public struct Roleplay
            {
                public string OfficerName;
                public string OfficerNumber;
                public string OfficerUnit;
                public bool RealismEnabled;
            }

            public struct Ambient
            {
                public bool RespondingToCalls;
                public bool Patrolling;
                public bool TrafficStops;
                public bool PedCrimes;
            }

            public struct Extensions
            {
                public bool TurnWheels { get; set; }
            }

            public class RootObject
            {
                public bool CheckForUpdates { get; set; }
                public Roleplay Roleplay;
                public Ambient AmbientEvents;
                public Extensions Extensions;
                public Callouts Callouts = new Callouts();
            }

            public class Callouts
            {
                public int MinRadius { get; set; }
                public int MaxRadius { get; set; }


                public bool AircraftCrash { get; set; }
                public bool AnimalAttack { get; set; }
                public bool Assault { get; set; }
                public bool AttemptedMurder { get; set; }
                public bool AttemptedSuicide { get; set; }
                public bool BarFight { get; set; }
                public bool CivOnFire { get; set; }
                public bool DeadBody { get; set; }
                public bool DrugBust { get; set; }
                public bool DUI { get; set; }
                public bool IndecentExposure { get; set; }
                public bool Kidnapping { get; set; }
                public bool Littering { get; set; }
                public bool Loitering { get; set; }
                public bool MissingPerson { get; set; }
                public bool MVA { get; set; }
                public bool OfficerDown { get; set; }
                public bool Overdose { get; set; }
                public bool ParkingViolation { get; set; }
                public bool PedHitByVehicle { get; set; }
                public bool PersonWithWeapon { get; set; }
                public bool PrankCall { get; set; }
                public bool Pursuit { get; set; }
                public bool Robbery { get; set; }
                public bool SexOffender { get; set; }
                public bool Speeding { get; set; }
                public bool SuspiciousItem { get; set; }
                public bool Vandalism { get; set; }
                public bool VehicleFire { get; set; }

                public bool Forgery { get; set; }
                public bool IllegalDeal { get; set; }
                public bool Importing { get; set; }
                public bool TerroristAttack { get; set; }
                public bool TerroristPlot { get; set; }

                public bool GangActivity { get; set; }

                public bool PrisonBreak { get; set; }
                public bool PrisonRiot { get; set; }

                public bool AnimalVsVehicle { get; set; }
                public bool IllegalHunting { get; set; }
                public bool OverKillLimit { get; set; }
                public bool Poaching { get; set; }
            }
        }
    }
}
