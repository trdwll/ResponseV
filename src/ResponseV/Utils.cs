using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ResponseV
{
    internal class Utils
    {
        public static bool m_bCheckingEMS = false;
        public static bool m_bEMSOnScene = false;

        public static GameFiber m_CheckEMSFiber;

        public static T GetRandValue<T>(params T[] args)
        {
            return args[MathHelper.GetRandomInteger(args.Length - 1)];
        }

        public static bool GetRandBool()
        {
            return MathHelper.GetRandomDouble(0.0, 1.0) >= 0.5;
        }

        public static T[] MergeArrays<T>(T[] array1, T[] array2)
        {
            T[] tmp = new T[array1.Length + array2.Length];
            array1.CopyTo(tmp, 0);
            array2.CopyTo(tmp, array1.Length);

            return tmp;
        }

        public static bool IsNight()
        {
            TimeSpan now = Rage.World.TimeOfDay;
            return now.Hours >= 20 || now.Hours < 6;
        }

        public static bool IsDay()
        {
            return !IsNight();
        }

        public static void Notify(string Message)
        {
            Game.DisplayNotification($"Response~y~V~w~ {Message}");
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

        public static bool IsLSPDFRPluginRunning(string PluginName)
        {
            foreach (Assembly assembly in Functions.GetAllUserPlugins())
            {
                AssemblyName an = assembly.GetName();
                if (an.Name.ToLower() == PluginName.ToLower())
                {
                    return true;
                }
                // for some reason this fucking return doesn't return true when it should
                //return assembly.GetName().Name.ToLower() == PluginName.ToLower();
            }

            return false;
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
                            Notify($"{GetRandValue("EMS", "Med", "Medic", "RA")}-{MathHelper.GetRandomInteger(1, 100)} on scene Dispatch.");
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
