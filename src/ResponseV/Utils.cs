using System;

using LSPD_First_Response;
using BetterEMS.API;
using Rage.Native;
using Rage;

namespace ResponseV
{
    public class Utils
    {
        private static readonly Random RANDOM = new Random();

        public static int GetRandInt(int min, int max)
        {
            return RANDOM.Next(min, max);
        }

        public static T GetRandValue<T>(params T[] args)
        {
            return args[GetRandInt(0, args.Length)];
        }
    }

    public class LSPDFR
    { 
        public static void RequestBackup(Rage.Vector3 pos, int numOfUnits,
            EBackupResponseType responseType = EBackupResponseType.Code3, 
            EBackupUnitType unitType = EBackupUnitType.LocalUnit)
        {
            for (int i = 0; i < numOfUnits; i++)
            {
                LSPD_First_Response.Mod.API.Functions.RequestBackup(pos, responseType, unitType);
            }
        }

        public static void RequestEMS(Rage.Vector3 location)
        {
            EMSFunctions.RespondToLocation(location);
        }

        public class Radio
        {
            public static readonly string[] WE_HAVE = { "CITIZENS_REPORT_01", "CITIZENS_REPORT_02", "CITIZENS_REPORT_03", "CITIZENS_REPORT_04", "OFFICERS_REPORT_01", "OFFICERS_REPORT_02", "OFFICERS_REPORT_03", "WE_HAVE_01", "WE_HAVE_02" };
            public static readonly string[] GANG = { "CRIME_GANG_01", "CRIME_GANG_02", "CRIME_GANG_03", "CRIME_GANG_04", "CRIME_GANG_05" };
            public static readonly string[] OVERDOSE = { "OVERDOSE_01", "OVERDOSE_02", "OVERDOSE_03", "OVERDOSE_04", "OVERDOSE_05" };
            public static readonly string[] DUI = { "CRIME_DUI_01", "CRIME_DUI_02", "CRIME_DUI_03", "CRIME_DUI_04" };
            public static readonly string[] AIRCRAFT_CRASH = { "CRIME_AIRCRAFT_CRASH_01", "CRIME_AIRCRAFT_CRASH_02", "CRIME_AIRCRAFT_CRASH_03", "CRIME_HELICOPTER_DOWN_01", "CRIME_HELICOPTER_DOWN_02" };
            public static readonly string[] DEADBODY = { "DECEASED_PERSON_01", "DECEASED_PERSON_02" };
            public static readonly string[] OFFICER_DOWN = { "CRIME_OFFICER_DOWN_01", "CRIME_OFFICER_DOWN_02", "CRIME_OFFICER_DOWN_03", "CRIME_OFFICER_DOWN_04" };

            public static string getRandomSound(string[] sounds)
            {
                return Utils.GetRandValue(sounds);
            }
        }
    }

    public class Native
    {
        public static bool GetSafeCoordForPed(Vector3 Point, out Vector3 outPos)
        {
            return NativeFunction.Natives.GET_SAFE_COORD_FOR_PED<bool>(Point.X, Point.Y, Point.Z, true, out outPos, 16);
        }
    }
}
