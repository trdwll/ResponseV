using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ResponseV
{
    public class Utils
    {
        private static Random RANDOM = new Random();

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
            // night = between 2000 - 0600
            TimeSpan now = Rage.World.TimeOfDay;
            TimeSpan start = new TimeSpan(20, 0, 0);
            TimeSpan end = new TimeSpan(06, 0, 0);

            return ((now >= start) && (now <= end));
        }

        public static bool IsDay()
        {
            return !IsNight();
        }

        public static void Notify(string Message)
        {
            Rage.Game.DisplayNotification(Message);
        }
    }


    // Credits to Albo1125 for these extensions 
    public static class AlboExtensions
    {
        /// <summary>
        /// Cache the result of whether a vehicle is an ELS vehicle.
        /// </summary>
        private static Dictionary<Model, bool> vehicleModelELSCache = new Dictionary<Model, bool>();

        /// <summary>
        /// Determine whether the passed vehicle model is an ELS vehicle.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool VehicleModelIsELS(Model model)
        {
            try
            {
                if (vehicleModelELSCache.ContainsKey(model))
                {
                    return vehicleModelELSCache[model];
                }

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ELS")))
                {
                    // no ELS installation at all
                    vehicleModelELSCache.Add(model, false);
                    return false;
                }

                IEnumerable<string> elsFiles = Directory.EnumerateFiles(
                    Path.Combine(Directory.GetCurrentDirectory(), "ELS"),
                    $"{model.Name}.xml", SearchOption.AllDirectories);

                vehicleModelELSCache.Add(model, elsFiles.Any());
                return vehicleModelELSCache[model];
            }
            catch (Exception e)
            {
                Game.LogTrivial($"Failed to determine if a vehicle model '{model}' was ELS-enabled: {e}");
                return false;
            }
        }

        /// <summary>
        /// Determine whether the passed vehicle is an ELS vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public static bool VehicleModelIsELS(this Vehicle vehicle)
        {
            return VehicleModelIsELS(vehicle.Model);
        }
    }
}
