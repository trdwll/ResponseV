using System;
using LSPD_First_Response.Mod.API;
using System.Reflection;
using System.Windows.Forms;
using ResponseV.Ambient;
using Rage;

namespace ResponseV
{
    public class Main : Plugin
    {
        public static Logger MainLogger = new Logger();

        public static bool g_bBetterEMS;
        public static bool g_bArrestManager;
        public static bool g_bTrafficPolicer;

        public override void Initialize()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LSPDFR);
            Functions.OnOnDutyStateChanged += OnDutyStateChangedEvent;

            Rage.Game.DisplayNotification($"Response~y~V~w~ ~b~{Configuration.APPVERSION} ~w~by ~b~trdwll ~w~loaded successfully.");

            Rage.Game.RawFrameRender += OnRawFrameRender;

            string Area = GTAV.Extensions.GetAreaName(Rage.Game.LocalPlayer.Character.Position);
            string WelcomeMessage = $"Welcome {Configuration.config.Roleplay.OfficerName}, to the " + (Utils.IsNight() ? "night" : "day") + $" shift in {Area}. Stay safe out there.";
            Utils.Notify(WelcomeMessage);

            // TODO: update checking (turn off via config)
        }

        private void OnDutyStateChangedEvent(bool onDuty)
        {
            if (!onDuty) return;

            // TODO: clean up this fucking mess
            // I was thinking an array to iterate over, but what if someone wants to disable a callout? /shrug

            // Rage.GameFiber AmbientEvents = Rage.GameFiber.StartNew(new System.Threading.ThreadStart(AmbientEvent.Initialize), "AmbientEventsFiber");

            // Finished/RFC
            // Functions.RegisterCallout(typeof(Callouts.Any.Overdose));
            // Functions.RegisterCallout(typeof(Callouts.Any.CivOnFire));
            // Functions.RegisterCallout(typeof(Callouts.Any.IndecentExposure));
            // Functions.RegisterCallout(typeof(Callouts.Any.VehicleFire));
            // Functions.RegisterCallout(typeof(Callouts.Any.PersonWithWeapon));
            // Functions.RegisterCallout(typeof(Callouts.Any.OfficerDown));
            // Functions.RegisterCallout(typeof(Callouts.Any.Speeding));
            // Functions.RegisterCallout(typeof(Callouts.Any.DeadBody));
            // Functions.RegisterCallout(typeof(Callouts.Any.DUI));
            // Functions.RegisterCallout(typeof(Callouts.Fed.UnionDepository));

            //Functions.RegisterCallout(typeof(Callouts.Any.ParkingViolation));
            

            // In Progress
            Functions.RegisterCallout(typeof(Callouts.Any.PrankCall));
            //Functions.RegisterCallout(typeof(Callouts.Any.Robbery));

            // Fed - These require more work than other callouts as we spawn specific vehicles in specific places etc
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
            //Functions.RegisterCallout(typeof(Callouts.Any.Trespassing));

            // Nature
            //Functions.RegisterCallout(typeof(Callouts.Nature.IllegalHunting));
            //Functions.RegisterCallout(typeof(Callouts.Nature.Poaching));
            //Functions.RegisterCallout(typeof(Callouts.Nature.AnimalVsVehicle));
            //Functions.RegisterCallout(typeof(Callouts.Nature.OverKillLimit));
            //Functions.RegisterCallout(typeof(Callouts.Nature.AnimalCruelty));
            //Functions.RegisterCallout(typeof(Callouts.Nature.EndangeredSpecies));

            Utils.Notify("Response~y~V~w~ Please wait around 30 seconds before going on patrol/receiving calls to allow Response~y~V~w~ to load fully.");
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
            e.Graphics.DrawText($"ResponseV {Configuration.APPVERSION}", "Verdana", 10.0f, new System.Drawing.PointF(2f, 2f), System.Drawing.Color.White);
        }
    }
}
