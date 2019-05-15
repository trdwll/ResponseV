using System;
using LSPD_First_Response.Mod.API;
using System.Reflection;
using System.Windows.Forms;
using ResponseV.Ambient;
using Rage;
using System.Runtime.InteropServices;

namespace ResponseV
{
    internal class Main : Plugin
    {
        public static Logger MainLogger = new Logger();

        public static bool g_bBetterEMS;
        public static bool g_bArrestManager;
        public static bool g_bTrafficPolicer;

        private static bool m_UpdateAvailable;

        public override void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFR);
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEvent;

            Game.DisplayNotification($"Response~y~V~w~ ~b~{Updater.m_AppVersion?.ToString()} ~w~by ~b~trdwll ~w~loaded successfully.");

            Game.RawFrameRender += OnRawFrameRender;

            if (Configuration.config.Roleplay.RealismEnabled)
            {
                string Area = GTAV.Extensions.GetAreaName(Rage.Game.LocalPlayer.Character.Position);
                string WelcomeMessage = $"Welcome {Configuration.config.Roleplay.OfficerName}, to the " + (Utils.IsNight() ? "night" : "day") + $" shift in {Area}. Stay safe out there.";
                Game.DisplayNotification(WelcomeMessage);
            }

            

            if (Configuration.config.CheckForUpdates)
            {
                m_UpdateAvailable = Updater.CheckForUpdates();
            }

            if (Configuration.config.Extensions.TurnWheels)
            {
                TurnWheels();
            }
        }

        private void OnDutyStateChangedEvent(bool onDuty)
        {
            if (!onDuty) return;

            // TODO: clean up this fucking mess
            // I was thinking an array to iterate over, but what if someone wants to disable a callout? /shrug

            // Rage.GameFiber AmbientEvents = Rage.GameFiber.StartNew(new System.Threading.ThreadStart(AmbientEvent.Initialize), "AmbientEventsFiber");

            Functions.RegisterCallout(typeof(Callouts.Any.AircraftCrash));
            //Functions.RegisterCallout(typeof(Callouts.Any.AnimalAttack));
            //Functions.RegisterCallout(typeof(Callouts.Any.Assault));
            //Functions.RegisterCallout(typeof(Callouts.Any.AssaultOnOfficer));
            //Functions.RegisterCallout(typeof(Callouts.Any.AttemptedMurder));
            //Functions.RegisterCallout(typeof(Callouts.Any.AttemptedSuicide));
            //Functions.RegisterCallout(typeof(Callouts.Any.BarFight));
            //Functions.RegisterCallout(typeof(Callouts.Any.BeachParty));
            // Functions.RegisterCallout(typeof(Callouts.Any.DeadBody));
            //Functions.RegisterCallout(typeof(Callouts.Any.Drowning));
            //Functions.RegisterCallout(typeof(Callouts.Any.DrugBust));
            // Functions.RegisterCallout(typeof(Callouts.Any.DUI));
            //Functions.RegisterCallout(typeof(Callouts.Any.GangActivity));
            //Functions.RegisterCallout(typeof(Callouts.Any.GraffitiArtist));
            // Functions.RegisterCallout(typeof(Callouts.Any.IndecentExposure));
            //Functions.RegisterCallout(typeof(Callouts.Any.Kidnapping));
            //Functions.RegisterCallout(typeof(Callouts.Any.Littering));
            //Functions.RegisterCallout(typeof(Callouts.Any.Loitering));
            //Functions.RegisterCallout(typeof(Callouts.Any.MVA));
            // Functions.RegisterCallout(typeof(Callouts.Any.OfficerDown));
            Functions.RegisterCallout(typeof(Callouts.Any.Overdose));
            //Functions.RegisterCallout(typeof(Callouts.Any.Paparazzi));
            // Functions.RegisterCallout(typeof(Callouts.Any.ParkingViolation));
            //Functions.RegisterCallout(typeof(Callouts.Any.Party));
            //Functions.RegisterCallout(typeof(Callouts.Any.PedHitByVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Any.PedMissing));
            // Functions.RegisterCallout(typeof(Callouts.Any.PedOnFire));
            // Functions.RegisterCallout(typeof(Callouts.Any.PedWithWeapon));
            // Functions.RegisterCallout(typeof(Callouts.Any.PrankCall));
            //Functions.RegisterCallout(typeof(Callouts.Any.Pursuit));
            //Functions.RegisterCallout(typeof(Callouts.Any.Robbery));
            //Functions.RegisterCallout(typeof(Callouts.Any.SearchWarrant));
            //Functions.RegisterCallout(typeof(Callouts.Any.SexOffender));
            // Functions.RegisterCallout(typeof(Callouts.Any.SpeedingVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Any.StreetPerformerFight));
            //Functions.RegisterCallout(typeof(Callouts.Any.SuspiciousItem));
            //Functions.RegisterCallout(typeof(Callouts.Any.Trespassing));
            //Functions.RegisterCallout(typeof(Callouts.Any.Vandalism));
            // Functions.RegisterCallout(typeof(Callouts.Any.VehicleFire));


            // Fed - These require more work than other callouts as we spawn specific vehicles in specific places etc
            //Functions.RegisterCallout(typeof(Callouts.Fed.TerroristPlot));
            //Functions.RegisterCallout(typeof(Callouts.Fed.TerroristAttack));
            //Functions.RegisterCallout(typeof(Callouts.Fed.Importing));
            //Functions.RegisterCallout(typeof(Callouts.Fed.Forgery));
            //Functions.RegisterCallout(typeof(Callouts.Fed.IllegalDeal));
            // Functions.RegisterCallout(typeof(Callouts.Fed.UnionDepository));

            // LSSD
            //Functions.RegisterCallout(typeof(Callouts.LSSD.PrisonBreak));
            //Functions.RegisterCallout(typeof(Callouts.LSSD.PrisonRiot));

            // Nature
            //Functions.RegisterCallout(typeof(Callouts.Nature.IllegalHunting));
            //Functions.RegisterCallout(typeof(Callouts.Nature.Poaching));
            //Functions.RegisterCallout(typeof(Callouts.Nature.AnimalVsVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Nature.OverKillLimit));
            //Functions.RegisterCallout(typeof(Callouts.Nature.AnimalCruelty));
            //Functions.RegisterCallout(typeof(Callouts.Nature.EndangeredSpecies));

            Utils.Notify("Please wait around 30 seconds before going on patrol/receiving calls to allow Response~y~V~w~ to load fully.");
            // So we put it on a new thread so we can sleep for 20 seconds to allow other plugins to be loaded fully before we start messing with them
            GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(20000);

                g_bBetterEMS = Utils.IsLSPDFRPluginRunning("BetterEMS");
                g_bArrestManager = Utils.IsLSPDFRPluginRunning("Arrest Manager");
                g_bTrafficPolicer = Utils.IsLSPDFRPluginRunning("Traffic Policer");

                MainLogger.Log($"BetterEMS: {g_bBetterEMS}");
                MainLogger.Log($"Arrest Manager: {g_bArrestManager}");
                MainLogger.Log($"Traffic Policer: {g_bTrafficPolicer}");

                foreach (Assembly a in Functions.GetAllUserPlugins())
                {
                    MainLogger.Log($"{a.GetName().Name} loaded");
                }

                Utils.Notify("You can go on duty now. Thanks for waiting. :)");
            });
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

        private static void OnRawFrameRender(object sender, Rage.GraphicsEventArgs e)
        {
            e.Graphics.DrawText($"ResponseV v{Updater.m_AppVersion?.ToString()}", "Verdana", 10.0f, new System.Drawing.PointF(2f, 2f), System.Drawing.Color.White);

            if (m_UpdateAvailable)
            {
                e.Graphics.DrawText($"Version {Updater.m_VersionAvailable?.ToString()} Available!", "Verdana", 10.0f, new System.Drawing.PointF(2f, 12f), System.Drawing.Color.Orange);
            }
        }

        private void TurnWheels()
        {
            MainLogger.Log("TurnWheels: Initialized");
            float SteeringAngle = 0f;
            GameFiber.StartNew(delegate
            {
                for (;;)
                {
                    GameFiber.Yield();
                    try
                    {
                        Vehicle LastVehicle = Game.LocalPlayer.Character.LastVehicle;
                        if (LastVehicle && LastVehicle.Speed == 0f)
                        {
                            if (LastVehicle.SteeringAngle > 20f)
                            {
                                SteeringAngle = 40f;
                            }
                            else if (LastVehicle.SteeringAngle < -20f)
                            {
                                SteeringAngle = -40f;
                            }
                            else if (LastVehicle.SteeringAngle > 5f || LastVehicle.SteeringAngle < -5f)
                            {
                                SteeringAngle = LastVehicle.SteeringAngle;
                            }

                            LastVehicle.SteeringAngle = SteeringAngle;
                        }
                    }
                    catch (Exception ex)
                    {
                        MainLogger.Log($"TurnWheels: {ex.Message}");
                        Utils.CrashNotify();
                    }
                }
            });
        }
    }
}
