using System;

namespace ResponseV
{
    public class API
    {
        /// <summary>
        /// Get the version of ResponseV
        /// NOTE: This can only be ran when LSPDFR is running and ResponseV is loaded.
        /// </summary>
        /// <returns>Version</returns>
        public static Version GetVersionNumber()
        {
            return Updater.m_AppVersion;
        }


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
