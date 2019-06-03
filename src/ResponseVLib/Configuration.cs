using System.Windows.Forms;

namespace ResponseVLib
{
    public class Configuration
    {
        public static readonly string ConfigPath = $"{Application.StartupPath}\\Plugins\\LSPDFR\\ResponseV\\";
        public static readonly string CustomAudioPath = $"{ConfigPath}\\custom_audio_files\\";
        private static readonly string ConfigFile = $"{Application.StartupPath}\\Plugins\\LSPDFR\\ResponseV\\ResponseV.json";
        public static Cfg.RootObject config = Serialization.JSON.Deserialize.GetFromFile<Cfg.RootObject>(ConfigFile);

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

            public struct Plugins
            {
                public bool TurnWheels { get; set; }
                public bool KeepDoorOpen { get; set; }
                public bool BaitCar { get; set; }
            }

            public struct Customization
            {
                public string AnimalControlPedModels { get; set; }
                public string AnimalControlVehicleModels { get; set; }

                public string GameWardenPedModels { get; set; }
                public string GameWardenVehicleModels { get; set; }
            }

            public class RootObject
            {
                public bool CheckForUpdates { get; set; }
                public string UpdateChannel { get; set; }
                public Roleplay Roleplay;
                public Ambient AmbientEvents;
                public Plugins Plugins;
                public Callouts Callouts = new Callouts();
                public Customization Customization;
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
