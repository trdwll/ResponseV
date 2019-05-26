using Rage;
using System.Collections.Generic;
using System.Linq;


namespace ResponseV
{
    internal class Utils
    {
        public static bool m_bCheckingEMS = false;
        public static bool m_bEMSOnScene = false;

        public static GameFiber m_CheckEMSFiber;

        public static void Notify(string Message)
        {
            Game.DisplayNotification($"[Response~y~V~w~] {Message}");
        }

        public static void CrashNotify()
        {
            Game.DisplayNotification($"Response~y~V~w~ has had an error. Please report crash to developer.");
        }

        public static void NotifyDispatchTo(string Unit, string Message)
        {
            // NotifyPlayerTo("Player", "Respond to xxx for ...")
            // NotifyPlayerTo("Player", "We've received multiple calls regarding this.")

            Configuration.Cfg.Roleplay cfg = Configuration.config.Roleplay;
            if (Unit == "Player" && cfg.RealismEnabled)
            {
                Game.DisplayNotification($"Dispatch to {cfg.OfficerUnit}-{cfg.OfficerNumber}: {Message}");
            }
            else
            {
                Game.DisplayNotification($"{Message}");
            }
        }

        public static void NotifyPlayerTo(string Unit, string Message)
        {
            // NotifyPlayerTo("Dispatch", "I'll be code 4")

            Configuration.Cfg.Roleplay cfg = Configuration.config.Roleplay;
            if (Unit == "Dispatch" && cfg.RealismEnabled)
            {
                Game.DisplayNotification($"{cfg.OfficerUnit}-{cfg.OfficerNumber} to Dispatch: {Message}");
            }
            else
            {
                Game.DisplayNotification($"{Message}");
            }
        }

        public static void CheckEMSOnScene(Vector3 location, string CalloutName)
        {
            m_bEMSOnScene = false;
            m_bCheckingEMS = true;
            m_CheckEMSFiber = GameFiber.StartNew(delegate
            {
                while (!m_bEMSOnScene)
                {
                    GameFiber.Yield();
                    List<Vehicle> vehicles = Game.LocalPlayer.Character.GetNearbyVehicles(16).ToList();

                    // can't use break in a lambda :(
                    foreach (Vehicle v in vehicles)
                    {
                        if (v.Model.Name == "AMBULANCE")
                        {
                            m_bEMSOnScene = true;
                            Main.MainLogger.Log($"{CalloutName} Utils.CheckEMSOnScene: EMS is on scene");
                            Notify($"{ResponseVLib.Utils.GetRandValue("EMS", "Med", "Medic", "RA")}-{MathHelper.GetRandomInteger(1, 150)} on scene Dispatch.");
                            break;
                        }
                    }

                    if (m_bEMSOnScene)
                    {
                        if (m_CheckEMSFiber.IsAlive)
                        {
                            m_CheckEMSFiber.Abort();
                            Main.MainLogger.Log($"{CalloutName} Utils.CheckEMSOnScene: Aborted CheckEMSFiber");
                        }

                        break;
                    }
                    else
                    {
                        GameFiber.Sleep(10000);
                        Main.MainLogger.Log($"{CalloutName} Utils.CheckEMSOnScene: Checking for nearby EMS");
                    }
                    vehicles.Clear();
                }

            }, $"{CalloutName}CheckEMSFiber");

            Main.g_GameFibers.Add(m_CheckEMSFiber);
        }
    }
}
