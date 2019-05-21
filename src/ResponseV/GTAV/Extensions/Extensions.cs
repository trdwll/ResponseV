using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Rage;
using Rage.Native;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting;

namespace ResponseV.GTAV
{
    internal static class Extensions
    {
        public static readonly int[] BlackListedNodeTypes = new int[] { 0, 8, 9, 10, 12, 40, 42, 136 };

        public static int GetNearestNodeType(this Vector3 pos)
        {
            uint node_prop_p1;
            int found_node_type;
            bool get_property_success = NativeFunction.Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(pos.X, pos.Y, pos.Z, out node_prop_p1, out found_node_type);

            return get_property_success ? found_node_type : -1;
        }

        public static bool IsNodeSafe(this Vector3 pos)
        {
            return !BlackListedNodeTypes.Contains(GetNearestNodeType(pos));
        }

        public static bool IsPointOnWater(this Vector3 position)
        {
            float height;
            return NativeFunction.Natives.GET_WATER_HEIGHT<bool>(position.X, position.Y, position.Z, out height);
        }

        public static void RegisterHatedTargetsAroundPed(this Ped ped, float radius)
        {
            Rage.Native.NativeFunction.Natives.REGISTER_HATED_TARGETS_AROUND_PED(ped, radius);
        }

        public static float DistanceTo(this Vector3 start, Vector3 end)
        {
            return (end - start).Length();
        }

        public static void MakePedReadyToSwim(Ped p)
        {
            NativeFunction.Natives.SET_PED_DIES_IN_WATER(p, false);
            NativeFunction.Natives.SET_PED_MAX_TIME_IN_WATER(p, 10000);
            NativeFunction.Natives.SET_PED_MAX_TIME_UNDERWATER(p, 10000);
            NativeFunction.Natives.SET_PED_DIES_INSTANTLY_IN_WATER(p, false);
            NativeFunction.Natives.xF35425A4204367EC(p, true); // SET_PED_PATHS_WIDTH_PLANT or SET_PED_PATH_MAY_ENTER_WATER
            NativeFunction.Natives.SET_PED_PATH_PREFER_TO_AVOID_WATER(p, false);
        }

        public static Callout GetCurrentRunningCallout()
        {
            foreach (Callout callout in ScriptComponent.GetAllByType<Callout>())
            {
                if (callout != null && callout.AcceptanceState == CalloutAcceptanceState.Running)
                {
                    return callout;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Gets a random position inside the defined radius
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="radius">Radius</param>
        /// <returns>a random position inside the defined radius</returns>
        public static Vector3 AroundPosition(this Vector3 v3, float radius)
        {
            return v3 + new Vector3(MathHelper.GetRandomSingle(-radius, radius), MathHelper.GetRandomSingle(-radius, radius), 0.0f);
        }

        /// <summary>
        /// Returns the heading of the closest vehicle node
        /// </summary>
        /// <param name="v3"></param>
        /// <returns>the heading of the closest vehicle node</returns>
        public static float GetClosestVehicleNodeHeading(this Vector3 v3)
        {
            float outHeading;
            Vector3 outPosition;

            NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING(v3.X, v3.Y, v3.Z, out outPosition, out outHeading, 12, 0x40400000, 0);

            return outHeading;
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this ISpatial spatial, ISpatial towards)
        {
            return GetHeadingTowards(spatial, towards.Position);
        }

        /// <summary>
        /// Gets the heading towards a position
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsPosition">Position to face to</param>
        /// <returns>the heading towards a position</returns>
        public static float GetHeadingTowards(this ISpatial spatial, Vector3 towardsPosition)
        {
            return GetHeadingTowards(spatial.Position, towardsPosition);
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, Vector3 towardsPosition)
        {
            Vector3 directionFromEntityToPosition = (towardsPosition - position);
            directionFromEntityToPosition.Normalize();

            float heading = MathHelper.ConvertDirectionToHeading(directionFromEntityToPosition);
            return heading;
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, ISpatial towards)
        {
            return GetHeadingTowards(position, towards.Position);
        }

        /// <summary>
        /// Returns a value indicating if this Rage.Entity instance is playing the chosen animation
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="animationDictionary">Animation dictionary in which the animation is</param>
        /// <param name="animationName">Animation name</param>
        /// <returns>a value indicating if this Rage.Entity instance is playing the chosen animation</returns>
        public static bool IsPlayingAnimation(this Entity entity, AnimationDictionary animationDictionary, string animationName)
        {
            return NativeFunction.Natives.IS_ENTITY_PLAYING_ANIM<bool>(entity, (string)animationDictionary, animationName, 3);
        }

        /// <summary>
        /// Suicides this Rage.Ped instance with a weapon
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="weapon">Weapon to use</param>
        public static void SuicideWeapon(this Ped ped, WeaponAsset weapon)
        {
            ped.Inventory.GiveNewWeapon(weapon, 10, true);

            ped.Tasks.PlayAnimation("mp_suicide", "pistol", 8.0f, AnimationFlags.None);

            GameFiber.Wait(700);

            if (ped.IsPlayingAnimation("mp_suicide", "pistol"))
            {
                NativeFunction.Natives.SET_PED_SHOOTS_AT_COORD(ped, 0.0f, 0.0f, 0.0f, 0);

                GameFiber.Wait(1000);

                ped.Kill();
            }
        }


        /// <summary>
        /// Suicides this Rage.Ped instance with a pill
        /// </summary>
        /// <param name="ped"></param>
        public static void SuicidePill(this Ped ped)
        {
            ped.Tasks.PlayAnimation("mp_suicide", "pill", 1.336f, AnimationFlags.None);

            GameFiber.Wait(2250);

            if (ped.IsPlayingAnimation("mp_suicide", "pill"))
            {
                ped.Kill();
            }
        }
    }

    // Credits to Albo1125 for these extensions 
    internal static class AlboExtensions
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
