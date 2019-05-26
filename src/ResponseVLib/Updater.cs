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

            public struct SUpdateData
            {
                public bool CriticalUpdate;
                public SUpdatePatchNotes PatchNotes;
            }

            public struct SUpdatePatchNotes
            {
                public string[] Added, Updated, Fixed, Removed, Deprecated, Security;
            }
        }


        public static uint m_CriticalUpdateCount { get; internal set; }
        public static bool m_bIsCriticalUpdate { get; internal set; }
        public static bool m_bHasCriticalUpdateBetween { get; internal set; }
        public static uint m_UpdateSinceCount { get; internal set; }

        public static List<string> m_CriticalVersions = new List<string>();


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

                Update update = ResponseVLib.Serialization.JSON.Deserialize.GetFromJson<Update>(content);

                Version CurrentUpdate = new Version(update.CurrentUpdate);

                if (CurrentUpdate > AppVersion)
                {
                    UpdateAvailable = CurrentUpdate;

                    foreach (var f in update.Updates.ToList())
                    {
                        if (new Version(f.Key) < AppVersion)
                        {
                            update.Updates.Remove(f.Key);
                        }
                    }

                    // Get the count of updates between AppVersion and CurrentVersion
                    m_UpdateSinceCount = (uint)update.Updates.Count() - 1;

                    // Check if there are any critical updates between the your version and previous updates
                    foreach (var upd in update.Updates.OrderBy(v => new Version(v.Key)))
                    {
                        if (upd.Value.CriticalUpdate && upd.Key.ToString() != CurrentUpdate.ToString())
                        {
                            m_CriticalUpdateCount++;
                            m_bHasCriticalUpdateBetween = true;
                            m_CriticalVersions.Add(upd.Key.ToString());
                        }
                    }

                    // Check if the current update is a critical update
                    Update.SUpdateData value;
                    if (update.Updates.TryGetValue(CurrentUpdate.ToString(), out value))
                    {
                        m_bIsCriticalUpdate = value.CriticalUpdate;
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
