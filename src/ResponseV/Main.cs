using LSPD_First_Response.Mod.API;
using Rage;
using ResponseVLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ResponseV
{
    internal class Main : Plugin
    {
        public static Version s_AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static Logger MainLogger = new Logger(s_AppVersion);

        /** Plugin implementations */
        public static bool s_bBetterEMS;
        public static bool s_bArrestManager;
        public static bool s_bTrafficPolicer;

        public static bool s_bRealismEnabled = Configuration.config.Roleplay.RealismEnabled;

        public static List<GameFiber> s_GameFibers = new List<GameFiber>();

        private static bool s_UpdateAvailable;
        private static Version s_VersionAvailable;
        private static string s_UpdateChannel;

        public override void Initialize()
        {
            /** Events */
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFR);
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEvent;
            Game.RawFrameRender += OnRawFrameRender;

            /** If realism is enabled then we welcome the officer on duty and what not. */
            if (s_bRealismEnabled)
            {
                string Area = WorldZone.GetAreaName(WorldZone.GetArea(Game.LocalPlayer.Character.Position));
                string WelcomeMessage = $"Welcome {ResponseVLib.Configuration.config.Roleplay.OfficerName}, to the " + (ResponseVLib.World.IsNight() ? "night" : "day") + $" shift in {Area}. Stay safe out there.";
                Game.DisplayNotification(WelcomeMessage);
            }

            /** Update handling */
            s_UpdateChannel = ResponseVLib.Configuration.config.UpdateChannel.ToLower();
            if (ResponseVLib.Configuration.config.CheckForUpdates)
            {
                s_UpdateAvailable = Updater.CheckForUpdates($"https://trdwll.com/api/ResponseV/update/{s_UpdateChannel}/", s_AppVersion, out s_VersionAvailable);

                if (s_UpdateAvailable)
                {
                    MainLogger.Log("ResponseV Updater: " + (Updater.s_bIsCriticalUpdate ? "Critical update" : "Update") + $" ({s_UpdateChannel}) {s_VersionAvailable.ToString()} available.");

                    if (s_UpdateChannel != "stable")
                    {
                        MainLogger.Log($"You can download the latest {s_UpdateChannel} update at {Updater.s_DownloadURL}");
                    }
                    if (Updater.s_UpdateSinceCount > 0)
                    {
                        MainLogger.Log($"ResponseV Updater: There are also {Updater.s_UpdateSinceCount} updates since you last updated.");
                    }
                    if (Updater.s_bHasCriticalUpdateBetween)
                    {
                        MainLogger.Log($"ResponseV Updater: There are {Updater.s_CriticalUpdateCount} critical updates between your version {s_AppVersion.ToString()} and the current version {s_VersionAvailable.ToString()}.");

                        Updater.s_CriticalVersions.ForEach(version =>
                        {
                            MainLogger.Log($"ResponseV Updater: Version {version.ToString()} is a critical update.");
                        });

                        MainLogger.Log("The developer advises you to update since there are critical updates between your version and the current version.");
                    }
                }
            }

            /** Repair vehicles */
            {
                // NOTE: This has to be above loading of plugins since we're in a forloop and yielding, else this won't load.
                GameFiber fiber = GameFiber.StartNew(delegate
                {
                    for (;;)
                    {
                        GameFiber.Yield();

                        if (ResponseVLib.Utils.IsKeyDown(System.Windows.Forms.Keys.T, System.Windows.Forms.Keys.None))
                        {
                            ResponseVLib.Vehicle.RepairVehicle();
                        }

                        if (ResponseVLib.Utils.IsKeyDown(System.Windows.Forms.Keys.End, System.Windows.Forms.Keys.None))
                        {
                            Functions.StopCurrentCallout();
                        }
                    }
                });

                s_GameFibers.Add(fiber);
            }

            /** Plugins */
            {
                if (ResponseVLib.Configuration.config.Plugins.TurnWheels)
                {
                    Plugins.TurnWheels.TurnWheelsImpl();
                }
                if (ResponseVLib.Configuration.config.Plugins.KeepDoorOpen)
                {
                    Plugins.KeepDoorOpen.KeepDoorOpenImpl();
                }
                if (ResponseVLib.Configuration.config.Plugins.BaitCar)
                {
                    Plugins.BaitCar.BaitCarImpl();
                }
            }
        }

        private void OnDutyStateChangedEvent(bool onDuty)
        {
            if (!onDuty) return;


            // TODO: clean up this fucking mess
            // I was thinking an array to iterate over, but what if someone wants to disable a callout? /shrug

            // Rage.GameFiber AmbientEvents = Rage.GameFiber.StartNew(new System.Threading.ThreadStart(AmbientEvent.Initialize), "AmbientEventsFiber");

            // TODO: 
            /*
             * County:
             * Chicken Fighting
             * Cow Tipping
             * 
             * Any:
             * Dog Fighting
             */ 

            // Functions.RegisterCallout(typeof(Callouts.Any.AircraftCrash)); // needs more testing and work
            Functions.RegisterCallout(typeof(Callouts.Any.AnimalAttack)); // needs more testing and work
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
            //Functions.RegisterCallout(typeof(Callouts.Any.PedWithWeapon));
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

            // TODO: Add a BOLO callout
            // TODO: Add a bicycle stolen callout (near right side of map in the small community/mirror park)


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

            Utils.Notify($"Please wait around 30 seconds before going on patrol/receiving calls to allow {Utils.PluginPrefix} to load fully.");
            // So we put it on a new thread so we can sleep for 20 seconds to allow other plugins to be loaded fully before we start messing with them
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(20000);

                s_bBetterEMS = ResponseVLib.Utils.IsLSPDFRPluginRunning("BetterEMS");
                s_bArrestManager = ResponseVLib.Utils.IsLSPDFRPluginRunning("Arrest Manager");
                s_bTrafficPolicer = ResponseVLib.Utils.IsLSPDFRPluginRunning("Traffic Policer");

                MainLogger.Log($"BetterEMS: {s_bBetterEMS}");
                MainLogger.Log($"Arrest Manager: {s_bArrestManager}");
                MainLogger.Log($"Traffic Policer: {s_bTrafficPolicer}");

                foreach (Assembly a in Functions.GetAllUserPlugins())
                {
                    MainLogger.Log($"{a.GetName().Name} loaded");
                }

                Utils.Notify("You can go on duty now. Thanks for waiting. :)");
            }, "DutyFiber");

            s_GameFibers.Add(fiber);
        }

        public override void Finally()
        {
            MainLogger.Log("Shutting down.");

            foreach (GameFiber fiber in s_GameFibers)
            {
                if (fiber.IsAlive)
                {
                    MainLogger.Log($"Aborting GameFiber '{fiber.Name}'");
                    fiber.Abort();
                }
            }

            s_GameFibers.Clear();
        }

        public static Assembly LSPDFR(object sender, ResolveEventArgs e)
        {
            foreach (Assembly a in Functions.GetAllUserPlugins())
            {
                if (e.Name.ToLower().Contains(a.GetName().Name.ToLower())) return a;
            }
            return null;
        }

        private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
        {
            e.Graphics.DrawText($"ResponseV v{s_AppVersion?.ToString()}  ({s_UpdateChannel})", "Verdana", 8.0f, new System.Drawing.PointF(2f, 2f), System.Drawing.Color.White);

            if (s_UpdateAvailable)
            {
                e.Graphics.DrawText((Updater.s_bIsCriticalUpdate ? "Critical update" : "Update") + $" ({s_UpdateChannel}) {s_VersionAvailable.ToString()} available!", "Verdana", 8.0f, new System.Drawing.PointF(2f, 12f), System.Drawing.Color.Orange);
            }
        }
    }
}
