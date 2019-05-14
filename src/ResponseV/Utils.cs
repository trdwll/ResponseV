using LSPD_First_Response.Engine.Scripting;
using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using WorldZone = ResponseV.GTAV.WorldZone;

namespace ResponseV
{
    public class Utils
    {
        private static Random RANDOM = new Random();

        public static bool m_bCheckingEMS = false;
        public static bool m_bEMSOnScene = false;

        public static int GetRandInt(int min, int max)
        {
            return RANDOM.Next(min, max);
        }

        public static T GetRandValue<T>(params T[] args)
        {
            return args[GetRandInt(0, args.Length)];
        }

        public static bool GetRandBool()
        {
            return RANDOM.Next(2) == 0;
        }

        public static double GetRandDouble()
        {
            return RANDOM.NextDouble();
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
            Rage.Game.DisplayNotification(Message);
        }

        public static void CrashNotify()
        {
            Rage.Game.DisplayNotification($"Response~y~V~w~ has had an error. Please report to developer.");
        }

        public static void NotifyDispatchTo(string Message)
        {

        }

        public static void NotifyPlayerTo(string Message)
        {

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
            GameFiber CheckEMSFiber = GameFiber.StartNew(delegate
            {
                while (!m_bEMSOnScene)
                {
                    GameFiber.Yield();
                    List<Vehicle> vehicles = Game.LocalPlayer.Character.GetNearbyVehicles(16).ToList();

                    vehicles.ForEach(v =>
                    {
                        if (v.Model.Name == "AMBULANCE")
                        {
                            m_bEMSOnScene = true;
                            Main.MainLogger.Log($"{CalloutName} Utils.CheckEMSOnScene: EMS is on scene");
                        }
                    });

                    GameFiber.Sleep(10000);
                    Main.MainLogger.Log($"{CalloutName} Utils.CheckEMSOnScene: Checking for nearby EMS");
                    vehicles.Clear();
                }

            }, "CheckEMSFiber");

            if (m_bEMSOnScene && CheckEMSFiber.IsAlive)
            {
                CheckEMSFiber.Abort();
                Main.MainLogger.Log($"{CalloutName} Utils.CheckEMSOnScene: Aborted CheckEMSFiber");
            }
        }
    }
}
