using LSPD_First_Response;
using Rage.Native;
using Rage;
using static ResponseV.Enums;
using System;
using LSPD_First_Response.Mod.API;

namespace ResponseV
{
    internal static class LSPDFR
    {
        public static void RequestBackup(Rage.Vector3 pos, int numOfUnits,
            EBackupResponseType responseType = EBackupResponseType.Code3,
            EBackupUnitType unitType = EBackupUnitType.LocalUnit)
        {
            for (int i = 0; i < numOfUnits; i++)
            {
                Functions.RequestBackup(pos, responseType, unitType);
            }
        }

        public static void RequestEMS(Rage.Vector3 location)
        {
            if (Main.s_bBetterEMS)
            {
                BetterEMS.API.EMSFunctions.RespondToLocation(location, false);
            }
            else
            {
                Functions.RequestBackup(location, EBackupResponseType.Code3, EBackupUnitType.Ambulance);
            }
        }

        public static void RequestFire(Rage.Vector3 location)
        {
            if (Main.s_bBetterEMS)
            {
                BetterEMS.API.EMSFunctions.RespondToLocation(location, true);
            }
            else
            {
                Functions.RequestBackup(location, EBackupResponseType.Code3, EBackupUnitType.Firetruck);
            }
        }

        public static void RequestCoroner(Rage.Vector3 location)
        {
            if (Main.s_bArrestManager)
            {
                Utils.NotifyPlayerTo("Dispatch", ResponseVLib.Utils.GetRandValue("Send a coroner.", "We need a coroner."));
                Arrest_Manager.API.Functions.CallCoroner(location, true);
            }
        }

        public static void AnnounceVehicleDetails(this Vehicle vehicle)
        {
            GameFiber fiber = GameFiber.StartNew(delegate 
            {
                ResponseVLib.Vehicle.RandomizeLicensePlate(vehicle);
                GameFiber.Sleep(3000);

                string VehicleName = vehicle.Model.Name.ToUpper(); //GTAV.VehicleExtensions.DetermineVehicleName(vehicle);
                string SoundName = $"MODEL_{VehicleName}_01";

                Functions.PlayScannerAudio(SoundName);

                string VehicleColor = ResponseVLib.Vehicle.GetColors(vehicle).PrimaryColorName;
                string VehiclePlate = vehicle.LicensePlate;

                {
                    string DispatchString = $"The RP reported the vehicle as a ~o~{VehicleName} ~s~in ~o~{VehicleColor} ~s~color.";

                    if (ResponseVLib.Utils.GetRandBool())
                    {
                        DispatchString += $"~n~The RP also wrote down a plate ~o~{VehiclePlate}~s~.";
                    }

                    Utils.NotifyDispatchTo("Player", DispatchString);
                }

                Main.MainLogger.Log($"AnnounceVehicleDetails - Name: {VehicleName}, Color: {VehicleColor}, License Plate Full: {vehicle.LicensePlate}");

            }, "AnnounceVehicleDetailsFiber");

            Main.s_GameFibers.Add(fiber);
        }

        public class Radio
        {
            public static readonly string[] WE_HAVE = { "0x0ACF391D", "0x1D1D5DBA", "0x1F72E264", "0x07F7F36D", "0x0662DE99", "0x1823821B" };
            public static readonly string[] OFFICERS_REPORT = { "0x03E3086B", "0x1628ACF6", "0x128465AD" };

            // Crime
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
            public static readonly string[] COMBUSTABLE_MATERIAL_INCIDENT = { "0x0BD091D7", "0x11861D3A" };
            public static readonly string[] CRIME_ASSOCIATION = { "0x1F598CB7", "0x047A56F7", "0x123BF27A", "0x16813B05", "0x11807101" };
            public static readonly string[] CRIMINAL_ACTIVITY = { "0x1B755C5C", "0x1E7A2266", "0x1160C832", "0x112987C5" };
            public static readonly string[] DAMAGE_TO_PROPERTY = { "0x009DAE97", "0x13A2949E", "0x020A316D" };
            public static readonly string[] DANGEROUS_DRIVING = { "0x1B6C996A", "0x0129A4E4" };
            public static readonly string[] DEADBODY = { "0x19D032B4", "0x148F6836", "0x0658CBC9", "0x1567C297", "0x1103211D" };
            public static readonly string[] DISTURBANCE = { "0x06A8CC92", "0x14AEA8AC", "0x19F7733E", "0x146B2817" };
            public static readonly string[] DOMESTIC_DISTURBANCE = { "0x0BECC465", "0x1A19A0BE" };
            public static readonly string[] DOMESTIC_VIOLENCE = { "0x0533758F" };
            public static readonly string[] DRIVEBY_ATTACK = { "0x0AC6744D" };
            public static readonly string[] DRIVEBY_SHOOTING = { "0x1C66805E" };
            public static readonly string[] DRIVER_OUT_OF_CONTROL = { "0x05E10283", "0x037BFDBB" };
            public static readonly string[] DRUG_DEAL = { "0x0EB8D5D8", "0x15D9E419", "0x18F3AA4E", "0x043280CC" }; // [3] == narcotics trafficking
            public static readonly string[] DUI = { "0x1E54C392", "0x08BE9874", "0x021C8B2F", "0x139E2E26", "0x142D2F50", "0x02098AFD" };
            public static readonly string[] ENDANGERED_SPECIES_ATTACK = { "0x1B784403" };
            public static readonly string[] ENDANGERED_SPECIES_HUNTING = { "0x1AFDD2BB" };
            public static readonly string[] EXPLOSION = { "0x1BF8589E", "0x05AC6C07" };
            public static readonly string[] FELON_ON_LOOSE = { "0x17CA8289" };
            public static readonly string[] FIRE_ALARM = { "0x0B3DC516" };
            public static readonly string[] FIREARM_DISCHARGED_IN_PUBLIC = { "0x085D09EB" };
            public static readonly string[] FIREARMS_POSSESSION = { "0x0C2145BC" };
            public static readonly string[] GANG_ACTIVITY = { "0x18F48FFD" };
            public static readonly string[] GANG_DISTURBANCE = { "0x0A3A3012", "0x0B23B1DB", "0x1D721678" };
            public static readonly string[] GANG_VIOLENCE = { "0x0EB801DB", "0x0092E590" };
            public static readonly string[] GRAND_THEFT = { "0x11C84461", "0x14FA8AC6", "0x047A29C5" };
            public static readonly string[] GRAND_THEFT_AUTO = { "0x0A4D736D", "0x0AA4B41B", "0x1C0316D8", "0x008EDFEF" };
            public static readonly string[] GUNFIRE_REPORTED = { "0x1A7C3D10", "0x1D3E4296" };
            public static readonly string[] HIGH_RANKING_GANG_MEMBER_IN_TRANSIT = { "0x0920EE1F" };
            public static readonly string[] HIJACKED_AIRCRAFT = { "0x1CFA777C" };
            public static readonly string[] HIJACKED_VEHICLE = { "0x0E518420" };
            public static readonly string[] HIT_AND_RUN = { "0x0C46540B", "0x1A9E8577", "0x08B8E1B0" };
            public static readonly string[] HIT_AND_RUN_FELONY = { "0x0C46540B", "0x05D8072E", "0x09A9CED1", "0x1719A9B2" };
            public static readonly string[] HOLD_UP = { "0x162A5F2D" };
            public static readonly string[] HUNTING_WITHOUT_PERMIT = { "0x08CC0434", "0x19FEA699" };
            public static readonly string[] ILLEGAL_ACTIVITY = { "0x02EE6B4D", "0x09E0F933", "0x112987C5" };
            public static readonly string[] ILLEGAL_BURNING = { "0x1ED03998", "0x1802EBFB" };
            public static readonly string[] ILLEGALLY_PARKED_VEHICLE = { "0x0D4EA555", "0x1E1106DA", "0x18DA7C6C", "0x092E5D13" };
            public static readonly string[] INDECENT_EXPOSURE = { "0x15AE8C53", "0x196E53D3", "0x0232E557" };
            public static readonly string[] INJURED_CIVILIAN = { "0x1AF8CB2D" };
            public static readonly string[] KIDNAPPING = { "0x0AA06B74", "0x1C284E82", "0x14D87FE1", "0x184786C0" };
            public static readonly string[] KILLING_ANIMALS = { "0x0651B03C" };
            public static readonly string[] LOITERING = { "0x14C10DE0" };
            public static readonly string[] LOW_FLYING_AIRCRAFT = { "0x0ED60811", "0x1D91E58B" };
            public static readonly string[] MALICIOUS_DAMAGE_TO_PROPERTY = { "0x0E66D16B" };
            public static readonly string[] MALICIOUS_DAMAGE_TO_VEHICLE = { "0x13B65F4F" };
            public static readonly string[] MALICIOUS_MISCHIEF = { "0x0EA052C1", "0x16BD22FD", "0x08494618" };
            public static readonly string[] MOTORCYCLE_NO_HELMET = { "0x13D5D4E1", "0x139E947B", "0x0563B805" };
            public static readonly string[] MUGGING = { "0x195CFE4E" };
            public static readonly string[] MULTIPLE_INJURIES = { "0x03841223" };
            public static readonly string[] MVA = { "0x1E710726", "0x0829DA96", "0x0305104D" };
            public static readonly string[] NARCOTICS_ACTIVITY = { "0x060CE903" };
            public static readonly string[] NARCOTICS_IN_TRANSIT = { "0x0E83D176" };
            public static readonly string[] OBSTRUCTION_OF_JUSTICE = { "0x08AFAEFF", "0x15400820" };
            public static readonly string[] OFFICER_DOWN = { "0x01FDB341", "0x05AFFAAA", "0x06FE2FC0", "0x17C49ECF", "0x146BDE45", "0x143757B4", "0x1844124C" };
            public static readonly string[] OFFICER_INJURED = { "0x095FEC28" };
            public static readonly string[] OFFICER_IN_DANGER = { "0x0E2B57AB" };
            public static readonly string[] OFFICER_SHOT = { "0x0237C6BB" };
            public static readonly string[] OFFICER_WOUNDED = { "0x15E81134" };
            public static readonly string[] OFFICER_HIT_BY_VEHICLE = { "0x05861C4A" };
            public static readonly string[] OFFICER_NEEDS_ASSISTANCE = { "0x1AA91FB1", "0x04B233C3" };
            public static readonly string[] OFFICER_ON_FIRE = { "0x0D64C020", "0x1FADA4B3" };
            public static readonly string[] OFFICER_STABBED = { "0x16DAE516" };
            public static readonly string[] OFFICER_UNDER_FIRE = { "0x1EC3B6A6", "0x0B6ECE66", "0x0C669B19", "0x06D48FF4", "0x0802D252" };
            public static readonly string[] OFFICERS_DOWN = { "0x1FF1244F", "0x10A785B3" };
            public static readonly string[] OVERDOSE = { "0x0FF49D54", "0x1D563817", "0x053AC7E1", "0x195AF020", "0x1217E179" };
            public static readonly string[] PED_DOWN = { "0x084DB426" };
            public static readonly string[] PED_FLEEING_CRIME_SCENE = { "0x13FA7AA2" };
            public static readonly string[] PED_HIT_BY_VEHICLE = { "0x0BB09D49", "0x0F91B046", "0x1AD73B96", "0x071DD422", "0x0366CCA6" };
            public static readonly string[] PED_IN_STOLEN_VEHICLE = { "0x0C290C9F", "0x06ECE5BE" };
            public static readonly string[] PED_INVOLVED_ACCIDENT = { "0x1933F786" };
            public static readonly string[] PED_RESISTING_ARREST = { "0x04A12B61" };
            public static readonly string[] PED_RUNNING_RED_LIGHT = { "0x0BFCA53A" };
            public static readonly string[] PED_SHOOTING_AT_ANIMALS = { "0x0EC4903B" };
            public static readonly string[] PED_STEALING_VEHICLE = { "0x03CBA5DC" };
            public static readonly string[] PED_THREATENING_OFFICER_FIREARM = { "0x05A09EE6" };
            public static readonly string[] PED_TORTURING_ANIMAL = { "0x0BB248B5" };
            public static readonly string[] PED_TRANSPORTING_NARCOTICS = { "0x0CA3A65C", "0x02CFBABE" };
            public static readonly string[] PED_UNCONSCIOUS = { "0x0839AE65" };
            public static readonly string[] PED_UNCONSCIOUS_FEMALE = { "0x19D87B01" };
            public static readonly string[] PED_UNCONSCIOUS_MALE = { "0x106D54AF" };
            public static readonly string[] PED_WITH_FIREARM = { "0x0A578619", "0x1F9EB0A4", "0x15D4DD10" };
            public static readonly string[] PED_WITH_KNIFE = { "0x1AFE9278", "0x05BD27F9", "0x11B4BFE4", "0x12F6426C" };
            public static readonly string[] PIMPING_AND_SOLICITATION = { "0x1436A1CD" };
            public static readonly string[] POLICE_TRANSPORT_ATTACK = { "0x1A6FA4E7", "0x0831006A" };
            public static readonly string[] POSESSION_DRUGS = { "0x0B6E4141", "0x0C0EAE77", "0x0E4C71EB", "0x1A250AA4", "0x1E95927E", "0x17A244B2", "0x086B6730", "0x0403DE60", "0x1502C05F", "0x10140A8E", "0x0654770E" };
            public static readonly string[] PROTECTED_SPECIES_ATTACK = { "0x07F5B9BF", "0x19A71D22" };
            public static readonly string[] PUBLIC_INTOXICATION = { "0x1632E73A", "0x08698BA8", "0x19882DE5" };
            public static readonly string[] PUBLIC_NUISANCE = { "0x0CE561EB", "0x021ACC57" };
            public static readonly string[] PURSE_THEFT = { "0x07E5A217", "0x13CE79E5", "0x05649D12" };
            public static readonly string[] RECKLESS_DRIVER = { "0x06F0317A" };
            public static readonly string[] RESISTING_ARREST = { "0x04DE3185" };
            public static readonly string[] ROBBERY = { "0x0A677E12", "0x149A5278" };
            public static readonly string[] ROBBERY_FIREARM = { "0x146F5EF1" };
            public static readonly string[] SHOOTING = { "0x01AC508D", "0x1F5D8BEF" };
            public static readonly string[] SHOOTING_DWELLING = { "0x04FA0824", "0x1624AA79", "0x17176C4F" };
            public static readonly string[] SHOOTING_PROTECTED_BIRD = { "0x0CBF96A2", "0x1A94324F", "0x195A6FDB" };
            public static readonly string[] SHOOTING_WILDLIFE = { "0x03E5D09E" };
            public static readonly string[] SHOTS_FIRED = { "0x0EADE575", "0x054E27A4", "0x193ACF7B", "0x0684AA0E" };
            public static readonly string[] SHOOT_OUT = { "0x09B682AC" };
            public static readonly string[] SOLICITATION = { "0x0DFDDC04" };
            public static readonly string[] SOS_CALL = { "0x04F36BF2", "0x123D0686" };
            public static readonly string[] SPEEDING = { "0x00BA5D8E", "0x1C912AD5", "0x17561F60" };
            public static readonly string[] SPEEDING_FELONY = { "0x01C16921" };
            public static readonly string[] STABBING = { "0x1DDB481E", "0x1280B178", "0x10112C89" };
            public static readonly string[] STOLEN_AIRCRAFT = { "0x11D9B7A5", "0x045DD97F" };
            public static readonly string[] STOLEN_HELICOPTER = { "0x1D5AF1B0" };
            public static readonly string[] STOLEN_POLICE_VEHICLE = { "0x00EA9A63", "0x0FEBF849", "0x063C24EB", "0x073466F5" };
            public static readonly string[] STOLEN_VEHICLE = { "0x0BFAF610", "0x0E03A0AD", "0x1A6892EB", "0x09CEE854", "0x12D86A56", "0x013CC71E", "0x05258EF1", "0x126934E4" };
            public static readonly string[] STOLEN_VEHICLE_ATTEMPT = { "0x0EA0B0C9" };
            public static readonly string[] STRUCTURE_FIRE = { "0x1E35841E", "0x067D94A5" };
            public static readonly string[] SUSPICIOUS_ACTIVITY = { "0x092DAC4A" };
            public static readonly string[] SUSPICIOUS_PERSON = { "0x1F38B43F", "0x11BA9943" };
            public static readonly string[] SUSPICIOUS_VEHICLE = { "0x08C22EB1" };
            public static readonly string[] TAMPERING_WITH_VEHICLE = { "0x08C01409", "0x1868B35A" };
            public static readonly string[] TERRORIST_ACTIVITY = { "0x0B7D09B8", "0x01AD7615", "0x1C8DABD5", "0x117ED5B8" };
            public static readonly string[] THEFT = { "0x0536E1CD" };
            public static readonly string[] THREATENING_PHONE_CALL = { "0x04A45011", "0x14332F31" };
            public static readonly string[] TRESPASSING = { "0x05C3CC3B", "0x1071E197" };
            public static readonly string[] TRESPASSING_GOV_PROPERTY = { "0x11228580" };
            public static readonly string[] UNAUTHORIZED_HUNTING = { "0x0D93CE7F" };
            public static readonly string[] VEHICLE_EXPLOSION = { "0x00ACF28E" };
            public static readonly string[] VEHICLE_FIRE = { "0x0CF82D90" };
            public static readonly string[] VEHICLES_RACING = { "0x00FCB2AB", "0x12CE564E" };
            public static readonly string[] VEHICULAR_HOMICIDE = { "0x08687686" };
            public static readonly string[] VICIOUS_ANIMAL = { "0x03A4A365", "0x00029C1B" };
            public static readonly string[] WARRANT_ISSUED = { "0x09926678" };

            // Weapons
            public static readonly string[] RPG = { "0x1F6F3AA8", "0x01007DCB" };
            public static readonly string[] GRENADE_LAUNCHER = { "0x00FE11D6", "0x0EDCED90" };
            public static readonly string[] EXPLOSIVE = { "0x0B56F594", "0x1A8453EF", "0x01948EA6" };
            public static readonly string[] KNIFE = { "0x01A0F654", "0x100F1331" };
            public static readonly string[] MINIGUN = { "0x1B175AFC" };
            public static readonly string[] MACHINEGUN = { "0x1B950B8E", "0x16E501E6" };
            public static readonly string[] SUB_MACHINEGUN = { "0x1B685349", "0x16BF89F9" };
            public static readonly string[] SHOTGUN = { "0x1D8FB2BF", "0x10DCD959" };
            public static readonly string[] SNIPER_RIFLE = { "0x05F26EE0" };
            public static readonly string[] BAT = { "0x04CFB5C1", "0x15EF5805" };
            public static readonly string[] WEAPON = { "0x04FA024B", "0x174C66EE" };
            public static readonly string[] GUN = { "0x048A699A", "0x191092A7" };
            public static readonly string[] GAT = { "0x1B62CC5D", "0x062DE1F3" };
            public static readonly string[] SAWED_OFF_SHOTGUN = { "0x07E4EAE1", "0x15968645" };
            public static readonly string[] FIREARM = { "0x10B1C1E8", "0x164D0D1F" };
            public static readonly string[] ASSAULT_RIFLE = { "0x087FD323" };
            public static readonly string[] ASSAULT_SHOTGUN = { "0x0872CE5F", "0x133223DD" };

            // Speeds
            public static readonly string DOING_40 = "0x1CCDF2A2";
            public static readonly string DOING_50 = "0x13559E06";
            public static readonly string DOING_60 = "0x18545449";
            public static readonly string DOING_70 = "0x0EB2AEE4";
            public static readonly string DOING_80 = "0x1A1E8187";
            public static readonly string DOING_90 = "0x1F166F61";
            public static readonly string DOING_100 = "0x082AF3B3";
            public static readonly string DOING_100_PLUS = "0x10BF0162";

            // Misc
            public static readonly string[] ALL_IN_CUSTODY = { "0x0DAB6838", "0x1BD6048E", "0x08CC5E87" };

            // Ages
            public static readonly string AGE_YOUNG = "0x057E03E3";
            public static readonly string AGE_TEENAGER = "0x1D5268E6";
            public static readonly string AGE_EARLY_20 = "0x028E8E07";
            public static readonly string AGE_EARLY_30 = "0x18ECC893";
            public static readonly string AGE_EARLY_40 = "0x1F9A8632";
            public static readonly string AGE_EARLY_50 = "0x00617E13";
            public static readonly string AGE_LATE_20 = "0x0F4F6D73";
            public static readonly string AGE_LATE_30 = "0x1A9DE131";
            public static readonly string AGE_LATE_40 = "0x1C74AC5A";
            public static readonly string AGE_LATE_50 = "0x11BC4434";
            public static readonly string AGE_MID_20 = "0x028E8E07";
            public static readonly string AGE_MID_30 = "0x198138E7";
            public static readonly string AGE_MID_40 = "0x16B57506";
            public static readonly string AGE_MID_50 = "0x164F5F0E";
            public static readonly string AGE_ELDERLY = "0x09B1D180";

            // Ped Build
            public static readonly string BUILD_ATHLETIC = "BUILD_ATHLETIC_01";
            public static readonly string BUILD_AVERAGE = "BUILD_AVERAGE_01";
            public static readonly string BUILD_HEAVY = "BUILD_HEAVY_01";
            public static readonly string BUILD_MUSCULAR = "BUILD_MUSCULAR_01";
            public static readonly string BUILD_OBESE = "BUILD_OBESE_01";
            public static readonly string BUILD_SLENDER = "BUILD_SLENDER_01";
            public static readonly string BUILD_THIN = "BUILD_THIN_01";

            // Dispatch respond
            public static readonly string[] RESPOND_CODE2 = { "0x00B0B158", "0x0B6786C0", "0x1CB5A95C", "RESPOND_CODE_2" };
            public static readonly string[] RESPOND_CODE3 = { "0x0DD15A9F", "0x0E9C9C35", "0x1917312B", "RESPOND_CODE_3" };
            public static readonly string[] RESPOND_CODE99 = { "0x09BEB4C8", "0x067E6E48" };
            public static readonly string[] RESPOND_CODE99_EMERGENCY = { "0x1B01574D", "0x113C43C4" };


            // ew
            public static string GetSpeedSound(uint Speed)
            {
                if (Speed >= 50 && Speed < 60)
                {
                    return DOING_50;
                }
                else if (Speed >= 60 && Speed < 70)
                {
                    return DOING_60;
                }
                else if (Speed >= 70 && Speed < 80)
                {
                    return DOING_70;
                }
                else if (Speed >= 80 && Speed < 90)
                {
                    return DOING_80;
                }
                else if (Speed >= 90 && Speed < 100)
                {
                    return DOING_90;
                }
                else if (Speed == 100 && Speed < 105)
                {
                    return DOING_100;
                }
                else if (Speed >= 106)
                {
                    return DOING_100_PLUS;
                }

                return DOING_40;
            }


            public static string GetRandomSound(string[] sounds)
            {
                return ResponseVLib.Utils.GetRandValue(sounds);
            }

            // Probably the worst way to do this, but it works for now
            public static string GetWeaponSound(WeaponHash type)
            {
                switch (type)
                {
                default:
                case WeaponHash.APPistol:
                case WeaponHash.Pistol:
                case WeaponHash.CombatPistol: // I know this is messy, but it works for now.
                case WeaponHash.Pistol50: return ResponseVLib.Utils.GetRandValue(ResponseVLib.Utils.MergeArrays(ResponseVLib.Utils.MergeArrays(GAT, FIREARM), ResponseVLib.Utils.MergeArrays(GUN, WEAPON)));
                case WeaponHash.AdvancedRifle:
                case WeaponHash.AssaultRifle:
                case WeaponHash.CarbineRifle: return ResponseVLib.Utils.GetRandValue(Radio.ASSAULT_RIFLE);
                case WeaponHash.AssaultSMG:
                case WeaponHash.MicroSMG: return ResponseVLib.Utils.GetRandValue(ResponseVLib.Utils.MergeArrays(GAT, SUB_MACHINEGUN));
                case WeaponHash.MG:
                case WeaponHash.CombatMG: return ResponseVLib.Utils.GetRandValue(MACHINEGUN);
                case WeaponHash.Bat: return ResponseVLib.Utils.GetRandValue(BAT);
                case WeaponHash.Knife: return ResponseVLib.Utils.GetRandValue(KNIFE);
                case WeaponHash.RPG: return ResponseVLib.Utils.GetRandValue(RPG);
                case WeaponHash.Minigun: return ResponseVLib.Utils.GetRandValue(MINIGUN);
                case WeaponHash.GrenadeLauncher: return ResponseVLib.Utils.GetRandValue(GRENADE_LAUNCHER);
                case WeaponHash.SawnOffShotgun: return ResponseVLib.Utils.GetRandValue(SAWED_OFF_SHOTGUN);
                case WeaponHash.AssaultShotgun: return ResponseVLib.Utils.GetRandValue(ASSAULT_SHOTGUN);
                case WeaponHash.PumpShotgun:
                case WeaponHash.BullpupShotgun: return ResponseVLib.Utils.GetRandValue(SHOTGUN);
                case WeaponHash.SniperRifle:
                case WeaponHash.HeavySniper: return ResponseVLib.Utils.GetRandValue(SNIPER_RIFLE);
                case WeaponHash.Grenade:
                case WeaponHash.SmokeGrenade: return ResponseVLib.Utils.GetRandValue(EXPLOSIVE);
                }
            }

            public static string GetResponseAudio(EResponse response)
            {
                switch (response)
                {
                default:
                case EResponse.R_NONE: return "";
                case EResponse.R_CODE1: return "RESPOND_CODE_1";
                case EResponse.R_CODE2: return "RESPOND_CODE_2";
                case EResponse.R_CODE3: return "RESPOND_CODE_3";
                case EResponse.R_CODE2OR3: return ResponseVLib.Utils.GetRandValue(ResponseVLib.Utils.MergeArrays(RESPOND_CODE2, RESPOND_CODE3));
                case EResponse.R_CODE3_99: return ResponseVLib.Utils.GetRandValue(RESPOND_CODE99);
                case EResponse.R_CODE3EMERGENCY: return ResponseVLib.Utils.GetRandValue(RESPOND_CODE99_EMERGENCY);
                case EResponse.R_RANDOM: return GetResponseAudio(RandomEnumValue<EResponse>());
                }
            }

            /// <summary>
            /// Return a string for Functions.PlayScannerAudioUsingPosition
            /// </summary>
            /// <param name="CallType"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public static string GetCalloutAudio(ECallType CallType, EResponse ResponseType = EResponse.R_NONE, params object[] args)
            {
                string ret = $"{GetRandomSound(WE_HAVE)} ";
                switch (CallType)
                {
                default:
                case ECallType.CT_AIRCRAFTCRASH: ret += (bool)args[0] ? $"{GetRandomSound(AIRCRAFT_CRASH)}" : $"{GetRandomSound(AIRCRAFT_HELICOPTER_CRASH)}"; break;
                case ECallType.CT_ANIMALATTACK: ret += $"{GetRandomSound(VICIOUS_ANIMAL)}"; break;
                case ECallType.CT_ASSAULT: ret += $"{GetRandomSound(ASSAULT)}"; break;
                case ECallType.CT_ASSAULTONOFFICER: ret += (bool)args[0] ? $"{GetRandomSound(ASSAULT_ON_OFFICER_KNIFE)}" : $"{GetRandomSound(ASSAULT_ON_OFFICER)}"; break;
                case ECallType.CT_ATTEMPTEDMUDER: ret += $"{GetRandomSound(ATTEMPTED_MURDER)}"; break;
                case ECallType.CT_ATTEMPTEDSUICIDE: ret += $"{GetRandomSound(ATTEMPTED_SUICIDE)}"; break;
                case ECallType.CT_BARFIGHT: ret += $"{GetRandomSound(ASSAULT)}"; break;
                case ECallType.CT_BEACHPARTY: ret += $""; break;
                case ECallType.CT_DEADBODY: ret += $"{GetRandomSound(DEADBODY)}"; break;
                case ECallType.CT_DROWNING: ret += $""; break;
                case ECallType.CT_DRUGBUST: ret += $"{GetRandomSound(DRUG_DEAL)}"; break; // ???
                case ECallType.CT_DUI: ret += $"{GetRandomSound(DUI)}"; break;
                case ECallType.CT_GANGACTIVITY:
                    #region Gang Activity
                    if ((uint)args[0] == 1)
                    {
                        ret += $"{GetRandomSound(GANG_ACTIVITY)}";
                    }
                    else if ((uint)args[0] == 2)
                    {
                        ret += $"{GetRandomSound(GANG_DISTURBANCE)}";
                    }
                    else if ((uint)args[0] == 3)
                    {
                        ret += $"{GetRandomSound(GANG_VIOLENCE)}";
                    }
                    #endregion Gang Activity
                    break;
                case ECallType.CT_GRAFFITIARTIST: ret += $""; break;
                case ECallType.CT_INDECENTEXPOSURE: ret += $"{GetRandomSound(INDECENT_EXPOSURE)}"; break;
                case ECallType.CT_KIDNAPPING: ret += $"{GetRandomSound(KIDNAPPING)}"; break;
                case ECallType.CT_LITTERING: ret += $""; break;
                case ECallType.CT_LOITERING: ret += $"{GetRandomSound(LOITERING)}"; break;
                case ECallType.CT_MVA: ret += $"{GetRandomSound(MVA)}"; break;
                case ECallType.CT_OFFICERDOWN:
                    #region Officer Down
                    if ((uint)args[0] == 1)
                    {
                        ret += $"{GetRandomSound(OFFICER_DOWN)}";
                    }
                    else if ((uint)args[0] == 2)
                    {
                        ret += $"{GetRandomSound(OFFICER_HIT_BY_VEHICLE)}";
                    }
                    else if ((uint)args[0] == 3)
                    {
                        ret += $"{GetRandomSound(OFFICER_INJURED)}";
                    }
                    else if ((uint)args[0] == 4)
                    {
                        ret += $"{GetRandomSound(OFFICER_SHOT)}";
                    }
                    else if ((uint)args[0] == 5)
                    {
                        ret += $"{GetRandomSound(OFFICER_STABBED)}";
                    }
                    else if ((uint)args[0] == 6)
                    {
                        ret += $"{GetRandomSound(OFFICER_UNDER_FIRE)}";
                    }
                    else if ((uint)args[0] == 7)
                    {
                        ret += $"{GetRandomSound(OFFICER_ON_FIRE)}";
                    }
                    else if ((uint)args[0] == 10)
                    {
                        ret += $"{GetRandomSound(OFFICERS_DOWN)}";
                    }
                    else
                    {
                        ret += "";
                    }
                    #endregion Officer Down
                    break;
                case ECallType.CT_OVERDOSE: ret += $"{GetRandomSound(OVERDOSE)}"; break;
                case ECallType.CT_PAPARAZZI: ret += $""; break;
                case ECallType.CT_PARKINGVIOLATION: ret += $"{GetRandomSound(ILLEGALLY_PARKED_VEHICLE)}"; break;
                case ECallType.CT_PARTY: ret += $""; break;
                case ECallType.CT_PEDHITBYVEHICLE: ret += $"{GetRandomSound(PED_HIT_BY_VEHICLE)}"; break;
                case ECallType.CT_PEDMISSING: ret += $""; break;
                case ECallType.CT_PEDONFIRE: ret += $""; break;
                case ECallType.CT_PEDWITHWEAPON: ret += ((bool)args[0] ? $"{GetRandomSound(PED_WITH_FIREARM)}" : $"{GetRandomSound(PED_WITH_KNIFE)}") + $"{LSPDFR.Radio.GetWeaponSound((WeaponHash)args[1])}"; break;
                case ECallType.CT_PRANKCALL: ret += $""; break;
                case ECallType.CT_PURSUIT: ret += $""; break;
                case ECallType.CT_ROBBERY: ret += $"{GetRandomSound(ROBBERY)}"; break;
                case ECallType.CT_RECKLESSDRIVING: ret += $"{GetRandomSound(RECKLESS_DRIVER)}"; break;
                case ECallType.CT_SEARCHWARRANT: ret += $"{GetRandomSound(WARRANT_ISSUED)}"; break;
                case ECallType.CT_SEXOFFENDER: ret += $""; break;
                case ECallType.CT_SPEEDINGVEHICLE: ret += $"{GetRandomSound(SPEEDING)} {GetSpeedSound((uint)args[0])}"; break;
                case ECallType.CT_STREETPERFORMERFIGHT: ret += $"{GetRandomSound(ASSAULT_BATTERY)}"; break;
                case ECallType.CT_SUSPICIOUSITEM: ret += $""; break;
                case ECallType.CT_TRESPASSING: ret = (bool)args[0] ? $"{GetRandomSound(TRESPASSING_GOV_PROPERTY)}" : $"{GetRandomSound(TRESPASSING)}"; break;
                case ECallType.CT_VANDALISM: ret = $"{GetRandomSound(DAMAGE_TO_PROPERTY)}"; break;
                case ECallType.CT_VEHICLEFIRE: ret = $"{GetRandomSound(VEHICLE_FIRE)}"; break;
                }

                ret += $" IN_OR_ON_POSITION {GetResponseAudio(ResponseType)}";
                return ret;
            }

            /// <summary>
            /// Get the string for CalloutMessage
            /// </summary>
            /// <param name="CallType">The call that you're wanting to get the string for</param>
            /// <param name="args"></param>
            /// <returns></returns>
            public static string GetCallStringFromEnum(ECallType CallType, params object[] args)
            {
                switch (CallType)
                {
                default:
                case ECallType.CT_AIRCRAFTCRASH: return (ResponseVLib.Utils.GetRandBool() ? ResponseVLib.Utils.GetRandValue("an Aircraft", "an Airplane") : "a Helicopter") +" Crash";
                case ECallType.CT_ANIMALATTACK: return ResponseVLib.Utils.GetRandValue("an Animal Attack", "Vicious Animal", "Vicious Animal on the Loose");
                case ECallType.CT_ASSAULT: return "an Assault";
                case ECallType.CT_ASSAULTONOFFICER: return "an Assault on an Officer";
                case ECallType.CT_ATTEMPTEDMUDER: return "an Attempted Murder";
                case ECallType.CT_ATTEMPTEDSUICIDE: return "an Attempted Suicide";
                case ECallType.CT_BARFIGHT: return "a Bar Fight";
                case ECallType.CT_BEACHPARTY: return "a Beach Party";
                case ECallType.CT_DEADBODY: return ResponseVLib.Utils.GetRandValue("a Dead Body", "a Dead Person", "a Deceased Person");
                case ECallType.CT_DROWNING: return ResponseVLib.Utils.GetRandValue("a Drowning", "a Possible Drowning");
                case ECallType.CT_DRUGBUST: return "a Drug Bust";
                case ECallType.CT_DUI: return ResponseVLib.Utils.GetRandValue("a DUI", "a Driver Under the Influence", "a Possible DUI", "a Possible Driver Under the Influence");
                case ECallType.CT_GANGACTIVITY:
                    #region Gang Activity
                    if ((uint)args[0] == 1)
                    {
                        return "Gang Activity";
                    }
                    else if ((uint)args[0] == 2)
                    {
                        return "a Gang Disturbance";
                    }
                    else if ((uint)args[0] == 3)
                    {
                        return "Gang Violence";
                    }
                    return "";
                    #endregion Gang Activity
                case ECallType.CT_GRAFFITIARTIST: return "a Graffiti Artist";
                case ECallType.CT_INDECENTEXPOSURE: return "Indecent Exposure";
                case ECallType.CT_KIDNAPPING: return "a Kidnapping";
                case ECallType.CT_LITTERING: return "Littering";
                case ECallType.CT_LOITERING: return "Loitering";
                case ECallType.CT_MVA: return ResponseVLib.Utils.GetRandValue("a Motor Vehicle Accident", "a MVA", "a Vehicle Accident", "a Vehicle Collision", "a MVC", "a Motor Vehicle Collision");
                case ECallType.CT_OFFICERDOWN:
                    #region Officer Down
                    if ((uint)args[0] == 1)
                    {
                        return "an Officer Down";
                    }
                    else if ((uint)args[0] == 2)
                    {
                        return "an Officer Hit by a Vehicle";
                    }
                    else if ((uint)args[0] == 3)
                    {
                        return "an Officer Injured";
                    }
                    else if ((uint)args[0] == 4)
                    {
                        return "an Officer Shot";
                    }
                    else if ((uint)args[0] == 5)
                    {
                        return "an Officer Stabbed";
                    }
                    else if ((uint)args[0] == 6)
                    {
                        return "an Officer Under Fire";
                    }
                    else if ((uint)args[0] == 7)
                    {
                        return "an Officer on Fire";
                    }
                    else if ((uint)args[0] == 10)
                    {
                        return ResponseVLib.Utils.GetRandValue("Multiple Officers Down", "Officers Down");
                    }
                    return "";
                    #endregion Officer Down
                case ECallType.CT_OVERDOSE: return ResponseVLib.Utils.GetRandValue("an Overdose", "a Possible Overdose");
                case ECallType.CT_PAPARAZZI: return "paparazzi";
                case ECallType.CT_PARKINGVIOLATION: return "a parking violation";
                case ECallType.CT_PARTY: return "a party";
                case ECallType.CT_PEDHITBYVEHICLE: return "a person hit by vehicle";
                case ECallType.CT_PEDMISSING: return "a person missing";
                case ECallType.CT_PEDONFIRE: return "a Person on Fire";
                case ECallType.CT_PEDWITHWEAPON: return "a person with a weapon";
                case ECallType.CT_PRANKCALL: return GetCallStringFromEnum(Enums.RandomEnumValue<ECallType>());
                case ECallType.CT_PURSUIT: return "a pursuit";
                case ECallType.CT_RECKLESSDRIVING: return "a reckless driver";
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
