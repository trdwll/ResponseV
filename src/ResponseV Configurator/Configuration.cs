using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV_Configurator
{
    public class Configuration
    {
        public class Cfg
        {
            public class RootObject
            {
                public Roleplay Roleplay = new Roleplay();
                public Ambient Ambient = new Ambient();
                public Callouts Callouts = new Callouts();
                public Misc Misc = new Misc();
            }

            public class Roleplay
            {
                public bool Realism { get; set; }
                public string OfficerName { get; set; } = "Ofc. Johnson";
            }

            public class Ambient
            {
                public bool Call { get; set; }
                public bool Patrol { get; set; }
                public bool TrafficStop { get; set; }
                public bool PedestrianCrimes { get; set; }
            }

            public class Callouts
            {
                public int MinRadius { get; set; } = 500;
                public int MaxRadius { get; set; } = 1000;

                public Dictionary<string, bool> Features { get; set; } = new Dictionary<string, bool>()
                {
                    {"AircraftCrash", true},
                    {"AnimalAttack", true},
                    {"AnimalVsVehicle", true},
                    {"Assault", true},
                    {"AttemptedMurder", true},
                    {"AttemptedSuicide", true},
                    {"BarFight", true},
                    {"CivOnFire", true},
                    {"DeadBody", true},
                    {"DrugBust", true},
                    {"DUI", true},
                    {"Forgery", false},
                    {"GangActivity", true},
                    {"IllegalDeal", false},
                    {"IllegalHunting", true},
                    {"Importing", false},
                    {"IndecentExposure", true},
                    {"Kidnapping", true},
                    {"Littering", true},
                    {"Loitering", true},
                    {"MissingPerson", true},
                    {"MVA", true},
                    {"OfficerDown", true},
                    {"Overdose", true},
                    {"OverKillLimit", true},
                    {"ParkingViolation", true},
                    {"PedHitByVehicle", true},
                    {"PersonWithWeapon", true},
                    {"Poaching", true},
                    {"PrankCall", true},
                    {"PrisonBreak", true},
                    {"PrisonRiot", true},
                    {"Pursuit", true},
                    {"Robbery", true},
                    {"SexOffender", true},
                    {"Speeding", true},
                    {"SuspiciousItem", true},
                    {"TerroristAttack", false},
                    {"TerroristPlot", false},
                    {"Vandalism", true},
                    {"VehicleFire", true}
                };
            }

            public class Misc
            {
                public bool TrafficBackup { get; set; }
            }
        }
    }
}
