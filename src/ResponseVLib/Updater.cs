using System;
using System.Collections.Generic;
using System.Linq;

namespace ResponseVLib
{
    public static class Updater
    {
        internal class Update
        {
            public Dictionary<string, SUpdateData> Updates = new Dictionary<string, SUpdateData>();

            public string CurrentUpdate;
            public string CurrentUpdateLabel;
            public string UpdateChannel;

            public struct SUpdateData
            {
                public DateTime ReleaseDate;
                public bool CriticalUpdate;
                public string DownloadURL;
                public SUpdatePatchNotes PatchNotes;
            }

            public struct SUpdatePatchNotes
            {
                public string[] Added, Updated, Fixed, Removed, Deprecated, Security;
            }
        }


        public static uint s_CriticalUpdateCount { get; internal set; }
        public static bool s_bIsCriticalUpdate { get; internal set; }
        public static bool s_bHasCriticalUpdateBetween { get; internal set; }
        public static uint s_UpdateSinceCount { get; internal set; }
        public static string s_UpdateChannel { get; internal set; }
        public static string s_DownloadURL { get; internal set; }
        public static DateTime s_CurrentReleaseDate { get; internal set; }
        public static int s_DaysSinceUpdate { get { return (int)Math.Round((s_CurrentReleaseDate - s_AppVersionReleaseDate).TotalDays); } }
        private static DateTime s_AppVersionReleaseDate { get; set; }

        public static List<string> s_CriticalVersions = new List<string>();

        /// <summary>
        /// Check for updates
        /// </summary>
        /// <param name="URL">The url to check for an update from.</param>
        /// <param name="AppVersion">Your local application version</param>
        /// <param name="UpdateAvailable">The current version</param>
        /// <returns>true if an update is available else false</returns>
        public static bool CheckForUpdates(string URL, Version AppVersion, out Version UpdateAvailable)
        {
            UpdateAvailable = AppVersion;
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            try
            {
                System.Net.WebClient webClient = new System.Net.WebClient();
                System.IO.StreamReader reader = new System.IO.StreamReader(webClient.OpenRead(URL));
                string content = reader.ReadToEnd();

                Update update = Serialization.JSON.Deserialize.GetFromJson<Update>(content);
                s_UpdateChannel = update.UpdateChannel;

                Version CurrentUpdate = new Version(update.CurrentUpdate);

                if (CurrentUpdate > AppVersion)
                {
                    UpdateAvailable = CurrentUpdate;

                    // Remove versions that are older than the AppVersion
                    foreach (var f in update.Updates.ToList())
                    {
                        if (new Version(f.Key) < AppVersion)
                        {
                            update.Updates.Remove(f.Key);
                        }
                    }

                    // Get the count of updates between AppVersion and CurrentVersion
                    s_UpdateSinceCount = (uint)update.Updates.Count() - 1;

                    // Check if there are any critical updates between the your version and previous updates
                    foreach (var upd in update.Updates.OrderBy(v => new Version(v.Key)))
                    {
                        if (upd.Value.CriticalUpdate && upd.Key.ToString() != CurrentUpdate.ToString())
                        {
                            s_CriticalUpdateCount++;
                            s_bHasCriticalUpdateBetween = true;
                            s_CriticalVersions.Add(upd.Key.ToString());
                        }
                    }

                    // Check if the current update is a critical update
                    Update.SUpdateData value;
                    if (update.Updates.TryGetValue(CurrentUpdate.ToString(), out value))
                    {
                        s_bIsCriticalUpdate = value.CriticalUpdate;

                        s_DownloadURL = value.DownloadURL;
                        s_CurrentReleaseDate = value.ReleaseDate;
                    }

                    // Get the AppVersion's release date
                    Update.SUpdateData value2;
                    if (update.Updates.TryGetValue(AppVersion.ToString(), out value2))
                    {
                        s_AppVersionReleaseDate = value2.ReleaseDate;
                    }

                    return true;
                }
            }
            catch (System.Net.NetworkInformation.NetworkInformationException ex)
            {

            }

            return false;
        }
    }
}
