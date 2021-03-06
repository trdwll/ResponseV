﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rage;
using Rage.Native;

namespace ResponseVLib
{
    public static class Vehicle
    {
        private static readonly string[] s_BlacklistedVehiclesList = { };
        // Model.VehicleModels.Where(model => !model.IsBicycle || !model.IsBlimp || !model.IsBoat || !model.IsJetski || !model.IsPlane || !model.IsSubmarine || !model.IsTrailer || !model.IsTrain).ToArray();

        /// <summary>
        /// Repair the vehicle nearby
        /// </summary>
        /// <param name="Radius"></param>
        /// <param name="bPlayAnimation"></param>
        /// <param name="CompletionTime"></param>
        public static void RepairVehicle(float Radius = 5.0f, bool bPlayAnimation = true, uint CompletionTime = 15000)
        {
            Rage.Vehicle vehicle = (Rage.Vehicle)Rage.World.GetClosestEntity(Game.LocalPlayer.Character.Position, Radius, GetEntitiesFlags.ConsiderAllVehicles);

            if (vehicle.Exists())
            {
                RepairVehicle_Impl(vehicle, bPlayAnimation, CompletionTime);
            }
        }

        /// <summary>
        /// Repair the vehicle
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="bPlayAnimation"></param>
        /// <param name="CompletionTime"></param>
        public static void RepairVehicle(this Rage.Vehicle vehicle, bool bPlayAnimation = true, uint CompletionTime = 15000)
        {
            if (vehicle.Exists())
            {
                RepairVehicle_Impl(vehicle, bPlayAnimation, CompletionTime);
            }
        }

        internal static void RepairVehicle_Impl(Rage.Vehicle vehicle, bool bPlayAnimation, uint CompletionTime)
        {
            Rage.Ped ped = Game.LocalPlayer.Character;

            if (Model.VehicleModels.Where(model => !model.IsAmphibious || !model.IsBicycle || !model.IsBlimp || !model.IsBoat || !model.IsJetski || !model.IsPlane || !model.IsSubmarine || !model.IsTrailer || !model.IsTrain).Contains(vehicle.Model)) return;

            if (!ped.Exists() || !ped.IsOnFoot || vehicle.EngineHealth > 400.0f || !vehicle.Exists() || Vector3.Distance(ped.Position, vehicle.GetBonePosition("engine")) > 1.8f) return;

            bool bHoodExists = vehicle.Doors[4].IsValid();

            if (bHoodExists)
            {
                vehicle.Doors[4].Open(false);
            }

            if (bPlayAnimation)
            {
                ped.Tasks.PlayAnimation(new AnimationDictionary("mini@repair"), "fixing_a_ped", 3.5f, AnimationFlags.None).WaitForCompletion((int)CompletionTime);
            }

            vehicle.EngineHealth = 1000.0f;

            if (bHoodExists)
            {
                vehicle.Doors[4].Close(false);
            }
        }

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
        public static bool VehicleModelIsELS(this Rage.Vehicle vehicle)
        {
            return VehicleModelIsELS(vehicle.Model);
        }


        /// <summary>
        /// Randomize the license plate to avoid excessively frequent debug plates from showing. - by PeterU, modified by trdwll
        /// </summary>
        public static void RandomizeLicensePlate(this Rage.Vehicle vehicle)
        {
            vehicle.LicensePlate = $"{MathHelper.GetRandomInteger(0, 9).ToString()}" +
                $"{MathHelper.GetRandomInteger(0, 9).ToString()}" +
                $"{Convert.ToChar(Convert.ToInt32(Math.Floor(26 * MathHelper.GetRandomDouble(0, 1) + 65)))}" +
                $"{Convert.ToChar(Convert.ToInt32(Math.Floor(26 * MathHelper.GetRandomDouble(0, 1) + 65)))}" +
                $"{Convert.ToChar(Convert.ToInt32(Math.Floor(26 * MathHelper.GetRandomDouble(0, 1) + 65)))}" +
                $"{MathHelper.GetRandomInteger(0, 9).ToString()}" +
                $"{MathHelper.GetRandomInteger(0, 9).ToString()}" +
                $"{MathHelper.GetRandomInteger(0, 9).ToString()}";
        }

        /// <summary>
        /// Sets the color to this Rage.Vehicle instance
        /// </summary>
        /// <param name="v"></param>
        /// <param name="primaryColor">The primary color</param>
        /// <param name="secondaryColor">The secondary color</param>
        public static void SetColors(this Rage.Vehicle v, EPaint primaryColor, EPaint secondaryColor)
        {
            NativeFunction.Natives.SET_VEHICLE_COLOURS(v, (int)primaryColor, (int)secondaryColor);
        }

        /// <summary>
        /// VehicleColor and associated methods were created by alexguirre
        /// </summary>
        public struct VehicleColor
        {
            /// <summary>
            /// The primary color paint index 
            /// </summary>
            public EPaint PrimaryColor { get; set; }

            /// <summary>
            /// The secondary color paint index 
            /// </summary>
            public EPaint SecondaryColor { get; set; }

            /// <summary>
            /// Gets the primary color name
            /// </summary>
            public string PrimaryColorName
            {
                get { return GetColorName(PrimaryColor); }
            }
            /// <summary>
            /// Gets the secondary color name
            /// </summary>
            public string SecondaryColorName
            {
                get { return GetColorName(SecondaryColor); }
            }

            /// <summary>
            /// Gets the color name
            /// </summary>
            /// <param name="paint">Color to get the name from</param>
            /// <returns></returns>
            public string GetColorName(EPaint paint)
            {
                string name = Enum.GetName(typeof(EPaint), paint);
                return name.Replace("_", " ");
            }
        }

        public enum EPaint
        {
            /* CLASSIC|METALLIC */
            Black = 0,
            Carbon_Black = 147,
            Graphite = 1,
            Anhracite_Black = 11,
            Black_Steel = 2,
            Dark_Steel = 3,
            Silver = 4,
            Bluish_Silver = 5,
            Rolled_Steel = 6,
            Shadow_Silver = 7,
            Stone_Silver = 8,
            Midnight_Silver = 9,
            Cast_Iron_Silver = 10,
            Red = 27,
            Torino_Red = 28,
            Formula_Red = 29,
            Lava_Red = 150,
            Blaze_Red = 30,
            Grace_Red = 31,
            Garnet_Red = 32,
            Sunset_Red = 33,
            Cabernet_Red = 34,
            Wine_Red = 143,
            Candy_Red = 35,
            Hot_Pink = 135,
            Pfister_Pink = 137,
            Salmon_Pink = 136,
            Sunrise_Orange = 36,
            Orange = 38,
            Bright_Orange = 138,
            Gold = 37,
            Bronze = 90,
            Yellow = 88,
            Race_Yellow = 89,
            Dew_Yellow = 91,
            Green = 139,
            Dark_Green = 49,
            Racing_Green = 50,
            Sea_Green = 51,
            Olive_Green = 52,
            Bright_Green = 53,
            Gasoline_Green = 54,
            Lime_Green = 92,
            Hunter_Green = 144,
            Securiror_Green = 125,
            Midnight_Blue = 141,
            Galaxy_Blue = 61,
            Dark_Blue = 62,
            Saxon_Blue = 63,
            Blue = 64,
            Bright_Blue = 140,
            Mariner_Blue = 65,
            Harbor_Blue = 66,
            Diamond_Blue = 67,
            Surf_Blue = 68,
            Nautical_Blue = 69,
            Racing_Blue = 73,
            Ultra_Blue = 70,
            Light_Blue = 74,
            Police_Car_Blue = 127,
            Epsilon_Blue = 157,
            Chocolate_Brown = 96,
            Bison_Brown = 101,
            Creek_Brown = 95,
            Feltzer_Brown = 94,
            Maple_Brown = 97,
            Beechwood_Brown = 103,
            Sienna_Brown = 104,
            Saddle_Brown = 98,
            Moss_Brown = 100,
            Woodbeech_Brown = 102,
            Straw_Brown = 99,
            Sandy_Brown = 105,
            Bleached_Brown = 106,
            Schafter_Purple = 71,
            Spinnaker_Purple = 72,
            Midnight_Purple = 142,
            Metallic_Midnight_Purple = 146,
            Bright_Purple = 145,
            Cream = 107,
            Ice_White = 111,
            Frost_White = 112,
            Pure_White = 134,
            Default_Alloy = 156,
            Champagne = 93,


            /* MATTE */
            Matte_Black = 12,
            Matte_Gray = 13,
            Matte_Light_Gray = 14,
            Matte_Ice_White = 131,
            Matte_Blue = 83,
            Matte_Dark_Blue = 82,
            Matte_Midnight_Blue = 84,
            Matte_Midnight_Purple = 149,
            Matte_Schafter_Purple = 148,
            Matte_Red = 39,
            Matte_Dark_Red = 40,
            Matte_Orange = 41,
            Matte_Yellow = 42,
            Matte_Lime_Green = 55,
            Matte_Green = 128,
            Matte_Forest_Green = 151,
            Matte_Foliage_Green = 155,
            Matte_Brown = 129,
            Matte_Olive_Darb = 152,
            Matte_Dark_Earth = 153,
            Matte_Desert_Tan = 154,


            /* METALS */
            Brushed_Steel = 117,
            Brushed_Black_Steel = 118,
            Brushed_Aluminum = 119,
            Pure_Gold = 158,
            Brushed_Gold = 159,


            /* CHROME */
            Chrome = 120,
        }

        /// <summary>
        /// Gets the primary and secondary colors of this instance of Rage.Vehicle
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static VehicleColor GetColors(this Rage.Vehicle v)
        {
            return UnsafeGetVehicleColors(v);
        }

        private static unsafe VehicleColor UnsafeGetVehicleColors(Rage.Vehicle vehicle)
        {
            int colorPrimaryInt;
            int colorSecondaryInt;

            ulong GetVehicleColorsHash = 0xa19435f193e081ac;
            NativeFunction.CallByHash<uint>(GetVehicleColorsHash, vehicle, &colorPrimaryInt, &colorSecondaryInt);

            VehicleColor colors = new VehicleColor();

            colors.PrimaryColor = (EPaint)colorPrimaryInt;
            colors.SecondaryColor = (EPaint)colorSecondaryInt;

            return colors;
        }
    }
}
