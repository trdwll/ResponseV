﻿using System;
using LSPD_First_Response.Mod.API;
using System.Reflection;
using ResponseV.Ambient;
using Rage;
using System.Collections.Generic;

using ResponseVLib;

namespace ResponseV
{
    internal class Main : Plugin
    {
        public static Logger MainLogger = new Logger();

        public static bool g_bBetterEMS;
        public static bool g_bArrestManager;
        public static bool g_bTrafficPolicer;

        public static List<GameFiber> g_GameFibers = new List<GameFiber>();

        public static readonly Version m_AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static bool m_UpdateAvailable;
        private static Version m_VersionAvailable;

        public override void Initialize()
        {
            //foreach (Model c in Model.VehicleModels.Where(v => v.IsCar && !v.IsLawEnforcementVehicle && !v.IsEmergencyVehicle && !v.IsBigVehicle).ToArray())
            //{
            //    MainLogger.Log($"Vehicle: {c.Name.ToUpper()}");
            //}

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFR);
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEvent;

            // Game.DisplayNotification($"Response~y~V~w~ ~b~{Updater.m_AppVersion?.ToString()} ~w~by ~b~trdwll ~w~loaded successfully.");

            Game.RawFrameRender += OnRawFrameRender;

            if (Configuration.config.Roleplay.RealismEnabled)
            {
                string Area = WorldZone.GetAreaName(WorldZone.GetArea(Rage.Game.LocalPlayer.Character.Position));
                string WelcomeMessage = $"Welcome {Configuration.config.Roleplay.OfficerName}, to the " + (ResponseVLib.World.IsNight() ? "night" : "day") + $" shift in {Area}. Stay safe out there.";
                Game.DisplayNotification(WelcomeMessage);
            }

            if (Configuration.config.CheckForUpdates)
            {
                m_UpdateAvailable = Updater.CheckForUpdates("https://trdwll.com/f/ResponseV.json", m_AppVersion, out m_VersionAvailable);

                MainLogger.Log("ResponseV Updater: " + (Updater.m_bIsCriticalUpdate ? "Critical update" : "Update") + $" {m_VersionAvailable.ToString()} available.");
                MainLogger.Log($"ResponseV Updater: There are also {Updater.m_UpdateSinceCount} updates since you last updated.");

                if (Updater.m_bHasCriticalUpdateBetween)
                {
                    MainLogger.Log($"ResponseV Updater: There are {Updater.m_CriticalUpdateCount} critical updates between your version {m_AppVersion.ToString()} and the current version {m_VersionAvailable.ToString()}.");

                    Updater.m_CriticalVersions.ForEach(version =>
                    {
                        MainLogger.Log($"ResponseV Updater: Version {version.ToString()} is a critical update.");
                    });
                }
            }

            /** Plugins */
            if (Configuration.config.Plugins.TurnWheels)
            {
                Plugins.TurnWheels.TurnWheelsImpl();
            }
            if (Configuration.config.Plugins.KeepDoorOpen)
            {
                Plugins.KeepDoorOpen.KeepDoorOpenImpl();
            }

            if (Configuration.config.Plugins.BaitCar)
            {
                Plugins.BaitCar.BaitCarImpl();
            }
        }

        private void OnDutyStateChangedEvent(bool onDuty)
        {
            if (!onDuty) return;


            // TODO: clean up this fucking mess
            // I was thinking an array to iterate over, but what if someone wants to disable a callout? /shrug

            // Rage.GameFiber AmbientEvents = Rage.GameFiber.StartNew(new System.Threading.ThreadStart(AmbientEvent.Initialize), "AmbientEventsFiber");

            // Functions.RegisterCallout(typeof(Callouts.Any.AircraftCrash)); // needs more testing and work
            // Functions.RegisterCallout(typeof(Callouts.Any.AnimalAttack)); // needs more testing and work
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
            // Functions.RegisterCallout(typeof(Callouts.Any.Overdose));
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

            Utils.Notify("Please wait around 30 seconds before going on patrol/receiving calls to allow Response~y~V~w~ to load fully.");
            // So we put it on a new thread so we can sleep for 20 seconds to allow other plugins to be loaded fully before we start messing with them
            GameFiber fiber = GameFiber.StartNew(delegate
            {
                GameFiber.Sleep(20000);

                g_bBetterEMS = ResponseVLib.Utils.IsLSPDFRPluginRunning("BetterEMS");
                g_bArrestManager = ResponseVLib.Utils.IsLSPDFRPluginRunning("Arrest Manager");
                g_bTrafficPolicer = ResponseVLib.Utils.IsLSPDFRPluginRunning("Traffic Policer");

                MainLogger.Log($"BetterEMS: {g_bBetterEMS}");
                MainLogger.Log($"Arrest Manager: {g_bArrestManager}");
                MainLogger.Log($"Traffic Policer: {g_bTrafficPolicer}");

                foreach (Assembly a in Functions.GetAllUserPlugins())
                {
                    MainLogger.Log($"{a.GetName().Name} loaded");
                }

                Utils.Notify("You can go on duty now. Thanks for waiting. :)");
            }, "DutyFiber");

            g_GameFibers.Add(fiber);
        }

        public override void Finally()
        {
            MainLogger.Log("Shutting down.");

            foreach (GameFiber fiber in g_GameFibers)
            {
                if (fiber.IsAlive)
                {
                    MainLogger.Log($"Aborting GameFiber '{fiber.Name}'");
                    fiber.Abort();
                }
            }

            g_GameFibers.Clear();
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
            e.Graphics.DrawText($"ResponseV v{m_AppVersion?.ToString()}", "Verdana", 8.0f, new System.Drawing.PointF(2f, 2f), System.Drawing.Color.White);

            if (m_UpdateAvailable)
            {
                e.Graphics.DrawText((Updater.m_bIsCriticalUpdate ? "Critical update" : "Update") + $" {m_VersionAvailable.ToString()} available!", "Verdana", 8.0f, new System.Drawing.PointF(2f, 12f), System.Drawing.Color.Orange);
            }
        }
    }
}
