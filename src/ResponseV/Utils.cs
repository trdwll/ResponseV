using System;

namespace ResponseV
{
    public class Utils
    {
        private static readonly Random RANDOM = new Random();

        public static int getRandInt(int min, int max)
        {
            return RANDOM.Next(min, max);
        }

        public static T getRandValue<T>(params T[] args)
        {
            return args[getRandInt(0, args.Length)];
        }

        public static void RequestBackup(Rage.Vector3 pos, int numOfUnits,
            LSPD_First_Response.EBackupResponseType responseType = LSPD_First_Response.EBackupResponseType.Code3, 
            LSPD_First_Response.EBackupUnitType unitType = LSPD_First_Response.EBackupUnitType.LocalUnit)
        {
            for (int i = 0; i < numOfUnits; i++)
            {
                LSPD_First_Response.Mod.API.Functions.RequestBackup(pos, responseType, unitType);
            }
        }

        public class Radio
        {
            public static readonly string[] WE_HAVE = { "CITIZENS_REPORT_01", "CITIZENS_REPORT_02", "CITIZENS_REPORT_03", "CITIZENS_REPORT_04",
                "OFFICERS_REPORT_01", "OFFICERS_REPORT_02", "OFFICERS_REPORT_03", "WE_HAVE_01", "WE_HAVE_02" };
            public static readonly string[] GANG = { "CRIME_GANG_01", "CRIME_GANG_02", "CRIME_GANG_03", "CRIME_GANG_04", "CRIME_GANG_05" };

            public static readonly string[] OVERDOSE = { "OVERDOSE_01", "OVERDOSE_02", "OVERDOSE_03", "OVERDOSE_04", "OVERDOSE_05" };

            public static string getRandomSound(string[] sounds)
            {
                return getRandValue(sounds);
            }
        }
    }
}
