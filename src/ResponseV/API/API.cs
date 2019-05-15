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
    }
}
