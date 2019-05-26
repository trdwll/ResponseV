using Rage;
using Rage.Native;

namespace ResponseVLib
{
    public static class Ped
    {
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
        public static void SuicideWeapon(this Rage.Ped ped, WeaponAsset weapon)
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
        public static void SuicidePill(this Rage.Ped ped)
        {
            ped.Tasks.PlayAnimation("mp_suicide", "pill", 1.336f, AnimationFlags.None);

            GameFiber.Wait(2250);

            if (ped.IsPlayingAnimation("mp_suicide", "pill"))
            {
                ped.Kill();
            }
        }

        public static void MakePedReadyToSwim(this Rage.Ped p)
        {
            NativeFunction.Natives.SET_PED_DIES_IN_WATER(p, false);
            NativeFunction.Natives.SET_PED_MAX_TIME_IN_WATER(p, 10000);
            NativeFunction.Natives.SET_PED_MAX_TIME_UNDERWATER(p, 10000);
            NativeFunction.Natives.SET_PED_DIES_INSTANTLY_IN_WATER(p, false);
            NativeFunction.Natives.xF35425A4204367EC(p, true); // SET_PED_PATHS_WIDTH_PLANT or SET_PED_PATH_MAY_ENTER_WATER
            NativeFunction.Natives.SET_PED_PATH_PREFER_TO_AVOID_WATER(p, false);
        }
    }
}
