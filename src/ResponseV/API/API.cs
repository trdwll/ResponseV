using System;

namespace ResponseV
{
    public class API
    {

        public class Callout
        {
            /// <summary>
            /// Request a game warden to the scene
            /// </summary>
            /// <param name="location"></param>
            public static void RequestGameWarden(Rage.Vector3 location)
            {
                Callouts.Roles.GameWarden.Request(location);
            }

            /// <summary>
            /// Request animal control to the scene
            /// </summary>
            /// <param name="location"></param>
            public static void RequestAnimalControl(Rage.Vector3 location)
            {
                Callouts.Roles.AnimalControl.Request(location);
            }

            public static void Notify1099(Rage.Vector3 location)
            {

            }
        }
    }
}
