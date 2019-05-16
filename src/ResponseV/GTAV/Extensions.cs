using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Rage;
using Rage.Native;

namespace ResponseV.GTAV
{
    internal static class Extensions
    {
        #region alexguirre
        // Credits to alexguirre
        public enum EWorldArea
        {
            Los_Santos = -289320599,
            Blaine_County = 2072609373,
            Oceana = 385094880,
        }

        public enum EWorldZone
        {
            NULL,
            PROL,
            AIRP,
            ALAMO,
            ALTA,
            ARMYB,
            BANHAMC,
            BANNING,
            BEACH,
            BHAMCA,
            BRADP,
            BRADT,
            BURTON,
            CALAFB,
            CANNY,
            CCREAK,
            CHAMH,
            CHIL,
            CHU,
            CMSW,
            CYPRE,
            DAVIS,
            DELBE,
            DELPE,
            DELSOL,
            DESRT,
            DOWNT,
            DTVINE,
            EAST_V,
            EBURO,
            ELGORL,
            ELYSIAN,
            GALFISH,
            golf,
            GRAPES,
            GREATC,
            HARMO,
            HAWICK,
            HORS,
            HUMLAB,
            JAIL,
            KOREAT,
            LACT,
            LAGO,
            LDAM,
            LEGSQU,
            LMESA,
            LOSPUER,
            MIRR,
            MORN,
            MOVIE,
            MTCHIL,
            MTGORDO,
            MTJOSE,
            MURRI,
            NCHU,
            NOOSE,
            OCEANA,
            PALCOV,
            PALETO,
            PALFOR,
            PALHIGH,
            PALMPOW,
            PBLUFF,
            PBOX,
            PROCOB,
            RANCHO,
            RGLEN,
            RICHM,
            ROCKF,
            RTRAK,
            SanAnd,
            SANCHIA,
            SANDY,
            SKID,
            SLAB,
            STAD,
            STRAW,
            TATAMO,
            TERMINA,
            TEXTI,
            TONGVAH,
            TONGVAV,
            VCANA,
            VESP,
            VINE,
            WINDF,
            WVINE,
            ZANCUDO,
            ZP_ORT,
            ZQ_UAR
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

        /// <summary>
        /// Returns the enum of the area
        /// </summary>
        /// <param name="position"></param>
        /// <returns>the name of the area</returns>
        public static EWorldArea GetArea(this Vector3 position)
        {
            return WorldZone.GetArea(position);
        }
        /// <summary>
        /// Returns the name of the zone
        /// </summary>
        /// <param name="position"></param>
        /// <returns>the name of the zone</returns>
        public static string GetAreaName(this Vector3 position)
        {
            return WorldZone.GetAreaName(WorldZone.GetArea(position));
        }

        /// <summary>
        /// Returns the enum of the zone
        /// </summary>
        /// <param name="position"></param>
        /// <returns>the name of the zone</returns>
        public static EWorldZone GetZone(this Vector3 position)
        {
            return WorldZone.GetZone(position);
        }

        /// <summary>
        /// Returns the name of the zone
        /// </summary>
        /// <param name="position"></param>
        /// <returns>the name of the zone</returns>
        public static string GetZoneName(this Vector3 position)
        {
            return WorldZone.GetZoneName(WorldZone.GetZone(position));
        }

        /// <summary>
        /// Gets the street name
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>the street name</returns>
        public static string GetStreetName(this Vector3 position)
        {
            return World.GetStreetName(position);
        }
        #endregion alexguirre
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
