using LSPD_First_Response;
using Rage.Native;
using Rage;

/*
 * Contains shorthand methods to do stuff with the LSPDFR API
 */
namespace ResponseV
{
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
            if (Main.g_bBetterEMS)
            {
                BetterEMS.API.EMSFunctions.RespondToLocation(location, false);
            }
            else
            {
                LSPD_First_Response.Mod.API.Functions.RequestBackup(location, EBackupResponseType.Code3, EBackupUnitType.Ambulance);
            }
        }

        public static void RequestFire(Rage.Vector3 location)
        {
            if (Main.g_bBetterEMS)
            {
                BetterEMS.API.EMSFunctions.RespondToLocation(location, true);
            }
            else
            {
                LSPD_First_Response.Mod.API.Functions.RequestBackup(location, EBackupResponseType.Code3, EBackupUnitType.Firetruck);
            }
        }

        public static void RequestCoroner(Rage.Vector3 location)
        {
            if (Main.g_bArrestManager)
            {
                Arrest_Manager.API.Functions.CallCoroner(location, true);
            }
        }

        /// <summary>
        /// Shoutout to khorio for the colors!
        /// </summary>
        public class CalloutStandardization
        {
            /// <summary>
            /// Set the color of the blip
            /// </summary>
            /// <param name="blip">The blip to set the color of</param>
            /// <param name="type">The color ((Blips default to yellow))</param>
            public static void SetStandardColor(Blip blip, BlipTypes type)
            {
                if (blip) NativeFunction.Natives.SET_BLIP_COLOUR(blip, (int)type);
            }

            /// <summary>
            /// Description
            /// <para>Enemy = Enemies  [red]</para>
            /// <para>Officers = Cops/Detectives/Commanders  [blue] (not gross system blue)</para>
            /// <para>Support = EMS/Coroner/ETC  [green]</para>
            /// <para>Civilians = Bystanders/Witnesses/broken down/etc  [orange]</para>
            ///  <para>Other = Animals/Obstacles/Rocks/etc  [purple]</para>
            /// </summary>
            public enum BlipTypes { Enemy = 1, Officers = 3, Support = 2, Civilians = 17, Other = 19 }

            public static void SetBlipScalePed(Blip blip)
            {
                if (blip) blip.Scale = 0.75f;
            }
        }

        public class Radio
        {
            public static readonly string[] WE_HAVE = { "CITIZENS_REPORT_01", "CITIZENS_REPORT_02", "CITIZENS_REPORT_03", "CITIZENS_REPORT_04", "WE_HAVE_01", "WE_HAVE_02" };
            public static readonly string[] OFFICERS_REPORT = { "OFFICERS_REPORT_01", "OFFICERS_REPORT_02", "OFFICERS_REPORT_03" };
            public static readonly string[] GANG = { "CRIME_GANG_01", "CRIME_GANG_02", "CRIME_GANG_03", "CRIME_GANG_04", "CRIME_GANG_05" };
            public static readonly string[] OVERDOSE = { "OVERDOSE_01", "OVERDOSE_02", "OVERDOSE_03", "OVERDOSE_04", "OVERDOSE_05" };
            public static readonly string[] DUI = { "CRIME_DUI_01", "CRIME_DUI_02", "CRIME_DUI_03", "CRIME_DUI_04" };
            public static readonly string[] AIRCRAFT_CRASH = { "CRIME_AIRCRAFT_CRASH_01", "CRIME_AIRCRAFT_CRASH_02", "CRIME_AIRCRAFT_CRASH_03", "CRIME_HELICOPTER_DOWN_01", "CRIME_HELICOPTER_DOWN_02" };
            public static readonly string[] DEADBODY = { "DECEASED_PERSON_01", "DECEASED_PERSON_02" };
            public static readonly string[] OFFICER_DOWN = { "CRIME_OFFICER_DOWN_01", "CRIME_OFFICER_DOWN_02", "CRIME_OFFICER_DOWN_03", "CRIME_OFFICER_DOWN_04" };
            public static readonly string[] OFFICERS_DOWN = { "CRIME_MULTIPLE_OFFICERS_DOWN_01", "CRIME_MULTIPLE_OFFICERS_DOWN_02" };
            public static readonly string[] OFFICER_SHOT = { "CRIME_OFFICER_HOMICIDE", "CRIME_OFFICER_HOMICIDE_02", "CRIME_OFFICER_FATALITY", "CRIME_OFFICER_SHOT", "CRIME_OFFICER_UNDER_FIRE", "CRIME_SHOTS_FIRED_OFFICER_01", "CRIME_SHOTS_FIRED_OFFICER_02", "CRIME_SHOTS_FIRED_OFFICER_03" };
            public static readonly string[] SPEEDING = { "CRIME_SPEEDING_01", "CRIME_SPEEDING_02", "CRIME_SPEEDING_03" };
            public static readonly string[] PARKING = { "CRIME_ILLEGAL_PARKING_01", "CRIME_ILLEGAL_PARKING_02", "PARKING_VIOLATION" };
            public static readonly string[] VEHICLE_FIRE = { "CRIME_CAR_FIRE_01", "CRIME_CAR_FIRE_02", "CRIME_CAR_FIRE_03" };
            public static readonly string[] ARMED_CAR_ROBBERY = { "CRIME_ARMED_CAR_ROBBERY_01", "CRIME_ARMED_CAR_ROBBERY_02" };

            public static string GetRandomSound(string[] sounds)
            {
                return Utils.GetRandValue(sounds);
            }

            // Probably the worst way to do this, but it works for now
            public static string GetWeaponSound(WeaponHash type)
            {
                string ret;
                switch (type)
                {
                    default:
                    case WeaponHash.APPistol:
                    case WeaponHash.Pistol:
                    case WeaponHash.CombatPistol:
                    case WeaponHash.Pistol50: ret = "FIREARM_01, FIREARM_02, GAT_01, GAT_02, GUN_01, GUN_02, WEAPON_01, WEAPON_02"; break;
                    case WeaponHash.AdvancedRifle:
                    case WeaponHash.AssaultRifle:
                    case WeaponHash.CarbineRifle: return "ASSAULT_RIFLE";
                    case WeaponHash.AssaultSMG:
                    case WeaponHash.MicroSMG: ret = "SUBMACHINE_GUN_01, SUBMACHINE_GUN_02, GAT_01, GAT_02"; break;
                    case WeaponHash.MG:
                    case WeaponHash.CombatMG: ret = "MACHINE_GUN_01, MACHINE_GUN_02"; break;
                    case WeaponHash.Bat: ret = "BAT_01, BAT_02"; break;
                    case WeaponHash.Knife: ret = "KNIFE_01, KNIFE_02"; break;
                    case WeaponHash.RPG: ret = "RPG_01, RPG_02"; break;
                    case WeaponHash.Minigun: return "MINIGUN";
                    case WeaponHash.GrenadeLauncher: ret = "GRENADE_LAUNCHER_01, GRENADE_LAUNCHER_02"; break;
                    case WeaponHash.SawnOffShotgun: ret = "SAWED_OFF_01, SAWED_OFF_02"; break;
                    case WeaponHash.AssaultShotgun: ret = "ASSAULT_SHOTGUN_01, ASSAULT_SHOTGUN_02"; break;
                    case WeaponHash.PumpShotgun:
                    case WeaponHash.BullpupShotgun: ret = "SHOTGUN_01, SHOTGUN_02"; break;
                    case WeaponHash.SniperRifle:
                    case WeaponHash.HeavySniper: return "SNIPER_RIFLE";
                    case WeaponHash.Grenade:
                    case WeaponHash.SmokeGrenade: ret = "EXPLOSIVE_01, EXPLOSIVE_02, EXPLOSIVE_03"; break;
                }

                return Utils.GetRandValue(ret.Split(','));
            }
        }
    }
}
