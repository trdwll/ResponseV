using System.Windows.Forms;

namespace ResponseV
{
    public class Configuration
    {
        public static readonly string APPVERSION = "v1.0.0";

        public static readonly string ConfigFile = $"{Application.StartupPath}\\Plugins\\LSPDFR\\ResponseV\\ResponseV.json";
        public static Cfg.RootObject config = Serialization.JSON.Deserialize.GetFromFile<Cfg.RootObject>(ConfigFile);

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
                public string OfficerName { get; set; }
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

            public class Misc
            {
                public bool TrafficBackup { get; set; }
            }
        }
    }
}
