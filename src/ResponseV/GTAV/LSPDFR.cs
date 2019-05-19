using LSPD_First_Response;
using Rage.Native;
using Rage;
using static ResponseV.Enums;
using System;

/*
 * Contains shorthand methods to do stuff with the LSPDFR API
 */
namespace ResponseV
{
    internal class LSPDFR
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
                Utils.NotifyPlayerTo("Dispatch", Utils.GetRandValue("send a coroner.", "we need a coroner."));
            }
        }

        public class Radio
        {
            public static readonly string[] WE_HAVE = { "CITIZENS_REPORT_01", "CITIZENS_REPORT_02", "CITIZENS_REPORT_03", "CITIZENS_REPORT_04", "WE_HAVE_01", "WE_HAVE_02" };
            public static readonly string[] OFFICERS_REPORT = { "OFFICERS_REPORT_01", "OFFICERS_REPORT_02", "OFFICERS_REPORT_03" };
            public static readonly string[] GANG = { "CRIME_GANG_01", "CRIME_GANG_02", "CRIME_GANG_03", "CRIME_GANG_04", "CRIME_GANG_05" };
            public static readonly string[] OVERDOSE = { "OVERDOSE_01", "OVERDOSE_02", "OVERDOSE_03", "OVERDOSE_04", "OVERDOSE_05" };
            public static readonly string[] DUI = { "CRIME_DUI_01", "CRIME_DUI_02", "CRIME_DUI_03", "CRIME_DUI_04" };
            public static readonly string[] AIRCRAFT_CRASH = { "CRIME_AIRCRAFT_CRASH_01", "CRIME_AIRCRAFT_CRASH_02", "CRIME_AIRCRAFT_CRASH_03" };
            public static readonly string[] HELICOPTER_CRASH = { "CRIME_HELICOPTER_DOWN_01", "CRIME_HELICOPTER_DOWN_02" };
            public static readonly string[] DEADBODY = { "DECEASED_PERSON_01", "DECEASED_PERSON_02" };
            public static readonly string[] OFFICER_DOWN = { "CRIME_OFFICER_DOWN_01", "CRIME_OFFICER_DOWN_02", "CRIME_OFFICER_DOWN_03", "CRIME_OFFICER_DOWN_04" };
            public static readonly string[] OFFICERS_DOWN = { "CRIME_MULTIPLE_OFFICERS_DOWN_01", "CRIME_MULTIPLE_OFFICERS_DOWN_02" };
            public static readonly string[] OFFICER_SHOT = { "CRIME_OFFICER_HOMICIDE", "CRIME_OFFICER_HOMICIDE_02", "CRIME_OFFICER_FATALITY", "CRIME_OFFICER_SHOT", "CRIME_OFFICER_UNDER_FIRE", "CRIME_SHOTS_FIRED_OFFICER_01", "CRIME_SHOTS_FIRED_OFFICER_02", "CRIME_SHOTS_FIRED_OFFICER_03" };
            public static readonly string[] SPEEDING = { "CRIME_SPEEDING_01", "CRIME_SPEEDING_02", "CRIME_SPEEDING_03" };
            public static readonly string[] PARKING = { "CRIME_ILLEGAL_PARKING_01", "CRIME_ILLEGAL_PARKING_02", "PARKING_VIOLATION" };
            public static readonly string[] VEHICLE_FIRE = { "CRIME_CAR_FIRE_01", "CRIME_CAR_FIRE_02", "CRIME_CAR_FIRE_03" };
            public static readonly string[] ARMED_CAR_ROBBERY = { "CRIME_ARMED_CAR_ROBBERY_01", "CRIME_ARMED_CAR_ROBBERY_02" };
            public static readonly string[] ANIMAL_ATTACK = { "CITIZENS_REPORT_01" };

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

            public static string GetCallSoundFromEnum_PrankCall(ECallType CallType)
            {
                string ret;
                switch (CallType)
                {
                default:
                case ECallType.CT_AIRCRAFTCRASH: ret = "blah"; break;
                case ECallType.CT_ANIMALATTACK: ret = "blah"; break;
                case ECallType.CT_ASSAULT: ret = "blah"; break;
                case ECallType.CT_ASSAULTONOFFICER: ret = "blah"; break;
                case ECallType.CT_ATTEMPTEDMUDER: ret = "blah"; break;
                case ECallType.CT_ATTEMPTEDSUICIDE: ret = "blah"; break;
                case ECallType.CT_BARFIGHT: ret = "blah"; break;
                case ECallType.CT_BEACHPARTY: ret = "blah"; break;
                case ECallType.CT_DEADBODY: ret = "blah"; break;
                case ECallType.CT_DROWNING: ret = "blah"; break;
                case ECallType.CT_DRUGBUST: ret = "blah"; break;
                case ECallType.CT_DUI: ret = "blah"; break;
                case ECallType.CT_GANGACTIVITY: ret = "blah"; break;
                case ECallType.CT_GRAFFITIARTIST: ret = "blah"; break;
                case ECallType.CT_INDECENTEXPOSURE: ret = "blah"; break;
                case ECallType.CT_KIDNAPPING: ret = "blah"; break;
                case ECallType.CT_LITTERING: ret = "blah"; break;
                case ECallType.CT_LOITERING: ret = "blah"; break;
                case ECallType.CT_MVA: ret = "blah"; break;
                case ECallType.CT_OFFICERDOWN: ret = "blah"; break;
                case ECallType.CT_OVERDOSE: ret = "blah"; break;
                case ECallType.CT_PAPARAZZI: ret = "blah"; break;
                case ECallType.CT_PARKINGVIOLATION: ret = "blah"; break;
                case ECallType.CT_PARTY: ret = "blah"; break;
                case ECallType.CT_PEDHITBYVEHICLE: ret = "blah"; break;
                case ECallType.CT_PEDMISSING: ret = "blah"; break;
                case ECallType.CT_PEDONFIRE: ret = "blah"; break;
                case ECallType.CT_PEDWITHWEAPON: ret = "blah"; break;
                case ECallType.CT_PRANKCALL: ret = "blah"; break;
                case ECallType.CT_PURSUIT: ret = "blah"; break;
                case ECallType.CT_ROBBERY: ret = "blah"; break;
                case ECallType.CT_SEARCHWARRANT: ret = "blah"; break;
                case ECallType.CT_SEXOFFENDER: ret = "blah"; break;
                case ECallType.CT_SPEEDINGVEHICLE: ret = "blah"; break;
                case ECallType.CT_STREETPERFORMERFIGHT: ret = "blah"; break;
                case ECallType.CT_SUSPICIOUSITEM: ret = "blah"; break;
                case ECallType.CT_TRESPASSING: ret = "blah"; break;
                case ECallType.CT_VANDALISM: ret = "blah"; break;
                case ECallType.CT_VEHICLEFIRE: ret = "blah"; break;
                }

                return ret;
            }

            public static string GetCallTypeFromEnum_PrankCall(ECallType CallType)
            {
                switch (CallType)
                {
                default:
                case ECallType.CT_AIRCRAFTCRASH: return "an aircraft crash";
                case ECallType.CT_ANIMALATTACK: return "an animal attack";
                case ECallType.CT_ASSAULT: return "an assault";
                case ECallType.CT_ASSAULTONOFFICER: return "an assault on an officer";
                case ECallType.CT_ATTEMPTEDMUDER: return "an attempted murder";
                case ECallType.CT_ATTEMPTEDSUICIDE: return "an attempted suicide";
                case ECallType.CT_BARFIGHT: return "a bar fight";
                case ECallType.CT_BEACHPARTY: return "a beach party";
                case ECallType.CT_DEADBODY: return Utils.GetRandValue("a dead body", "a dead person");
                case ECallType.CT_DROWNING: return "a drowning";
                case ECallType.CT_DRUGBUST: return "a drug bust";
                case ECallType.CT_DUI: return Utils.GetRandValue("a dui", "a driver under the influence");
                case ECallType.CT_GANGACTIVITY: return "gang activity";
                case ECallType.CT_GRAFFITIARTIST: return "a graffiti artist";
                case ECallType.CT_INDECENTEXPOSURE: return "indecent exposure";
                case ECallType.CT_KIDNAPPING: return "a kidnapping";
                case ECallType.CT_LITTERING: return "littering";
                case ECallType.CT_LOITERING: return "loitering";
                case ECallType.CT_MVA: return Utils.GetRandValue("a motor vehicle accident", "a mva", "a vehicle accident");
                case ECallType.CT_OFFICERDOWN: return Utils.GetRandValue("multiple officers down", "an officer down");
                case ECallType.CT_OVERDOSE: return Utils.GetRandValue("an overdose", "a possible overdose");
                case ECallType.CT_PAPARAZZI: return "paparazzi";
                case ECallType.CT_PARKINGVIOLATION: return "a parking violation";
                case ECallType.CT_PARTY: return "a party";
                case ECallType.CT_PEDHITBYVEHICLE: return "a person hit by vehicle";
                case ECallType.CT_PEDMISSING: return "a person missing";
                case ECallType.CT_PEDONFIRE: return "a person on fire";
                case ECallType.CT_PEDWITHWEAPON: return "a person with a weapon";
                case ECallType.CT_PRANKCALL: return GetCallTypeFromEnum_PrankCall(Enums.RandomEnumValue<ECallType>());
                case ECallType.CT_PURSUIT: return "a pursuit";
                case ECallType.CT_ROBBERY: return "a robbery";
                case ECallType.CT_SEARCHWARRANT: return "a search warrant";
                case ECallType.CT_SEXOFFENDER: return "a sex offender";
                case ECallType.CT_SPEEDINGVEHICLE: return "a speeding vehicle";
                case ECallType.CT_STREETPERFORMERFIGHT: return "a street performer fight";
                case ECallType.CT_SUSPICIOUSITEM: return "a suspicious item";
                case ECallType.CT_TRESPASSING: return "a trespassing";
                case ECallType.CT_VANDALISM: return "a vandalism";
                case ECallType.CT_VEHICLEFIRE: return "a vehicle on fire";
                }
            }
        }
    }
}
