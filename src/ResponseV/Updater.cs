using System;

namespace ResponseV
{
    internal class Updater
    {
        public static readonly Version m_AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static Version m_VersionAvailable;

        public static bool CheckForUpdates()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                Main.MainLogger.Log("Updater: No network available, please try again later.");
                return false;
            }

            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                Version WebVersion = new Version(new System.IO.StreamReader(webClient.OpenRead("https://trdwll.com/f/ResponseV.txt")).ReadToEnd());

                if (WebVersion > m_AppVersion)
                {
                    m_VersionAvailable = WebVersion;
                    return true;
                }
            }
            catch (System.Net.NetworkInformation.NetworkInformationException ex)
            {
                Main.MainLogger.Log(ex.Message, Logger.ELogLevel.LL_TRACE);
            }

            return false;
        }
    }
}
