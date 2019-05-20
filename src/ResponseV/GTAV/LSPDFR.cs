using LSPD_First_Response;
using Rage.Native;
using Rage;
using static ResponseV.Enums;
using System;

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
                Utils.NotifyPlayerTo("Dispatch", Utils.GetRandValue("send a coroner.", "we need a coroner."));
                Arrest_Manager.API.Functions.CallCoroner(location, true);
            }
        }

        public class Radio
        {
            public static readonly string[] WE_HAVE = { "0x0ACF391D", "0x1D1D5DBA", "0x1F72E264", "0x07F7F36D", "0x0662DE99", "0x1823821B" };
            public static readonly string[] OFFICERS_REPORT = { "0x03E3086B", "0x1628ACF6", "0x128465AD" };

            public static readonly string[] _1099 = { "0x02A890D4", "0x108A6C97", "0x029C50BE", "0x149B34B9" };
            public static readonly string[] ACCIDENT = { "0x0AC0DB4B" };
            public static readonly string[] AIRCRAFT_CRASH = { "0x04A51B94", "0x183EC2CD", "0x190D4464" };
            public static readonly string[] AIRCRAFT_HELICOPTER_CRASH = { "0x1DB63742", "0x1F6ABAA8" };
            public static readonly string[] ANIMAL_CRUELTY = { "0x12BA7967", "0x16DD41AE", "0x0383DAFA" };
            public static readonly string[] ANIMAL_KILLED = { "0x0BAACC73", "0x0F3A9393" };
            public static readonly string[] ARMED_AND_DANGEROUS = { "0x08E2DF46" };
            public static readonly string[] ARMED_ROBBERY = { "0x1E842DC8", "0x07B6402C", "0x19F9E4B3" };
            public static readonly string[] ARMORED_CAR_ROBBERY = { "0x0E8B5D1D", "0x1D40BA88" };
            public static readonly string[] ARSON = { "0x040FE61D", "0x124D0298" };
            public static readonly string[] ASSAULT = { "0x1E4332B2", "0x15AA986C", "0x058AC21E", "0x0458F5C8", "0x1009963F" };
            public static readonly string[] ASSAULT_BATTERY = { "0x0C4A5075", "0x1AF12DC3" };
            public static readonly string[] ASSAULT_DEADLY_WEAPON = { "0x0B1C964E", "0x0C3E8856", "0x1E1A2C0D", "0x032B7620", "0x1909F229" };
            public static readonly string[] ASSAULT_ON_OFFICER = { "0x1F80BD56", "0x05D1A54E", "0x162AEAAA", "0x11374E1D" };
            public static readonly string[] ASSAULT_ON_OFFICER_KNIFE = { "0x18674968" };
            public static readonly string[] ATTACK_ON_VEHICLE = { "0x0BCD234E", "0x05B29DBA" };
            public static readonly string[] ATTEMPTED_MURDER = { "0x0AFA345E", "0x1A5C9323", "0x137F395C" };
            public static readonly string[] ATTEMPTED_SUICIDE = { "0x1AE16488", "0x08A0C007", "0x037FB5C7", "0x0765BD91", "0x1141114A" };
            public static readonly string[] BANK_ROBBERY = { "0x0D703F9D", "0x1B249B06", "0x18C2D609", "0x08797576", "0x12590970" };
            public static readonly string[] BATTERY = { "0x13C5AA66", "0x039009FB" };
            public static readonly string[] BREAKING_AND_ENTERING = { "0x0FDFC35A", "0x1E81A09D", "0x061DEFD6", "0x13790A8D" };
            public static readonly string[] BURGLARY = { "0x065CDE1D" };
            public static readonly string[] CAR_FIRE = { "0x18138D5F", "0x065069D8", "0x13964464" };
            public static readonly string[] CIV_DOWN = { "0x0320C921", "0x00445298" };
            public static readonly string[] CIV_GSW = { "0x0382F686", "0x15439A09", "0x11579233" };
            public static readonly string[] CIV_NEEDS_ASSISTANCE = { "0x0B380179", "0x09EF3EE7" };
            public static readonly string[] CIV_ON_FIRE = { "0x0CCE003C" };
            public static readonly string[] CIVIL_DISTURBANCE = { "0x16A9B4AA" };






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
