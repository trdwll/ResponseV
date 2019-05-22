using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseV
{
    internal class Enums
    {
        public enum Callout
        {
            InPursuit,
            EnRoute,
            OnScene,
            Done
        };

        // used for when we have multiple outcomes for a callout like Officer Down
        public enum CalloutType
        {
            FailureToCheckIn,
            Stabbing,
            Shooting,
            Unknown
        }

        public enum Response
        {
            Code2,
            Code3,
            Silent
        }

        public enum ECallType
        {
            CT_AIRCRAFTCRASH,
            CT_ANIMALATTACK,
            CT_ASSAULT,
            CT_ASSAULTONOFFICER,
            CT_ATTEMPTEDMUDER,
            CT_ATTEMPTEDSUICIDE,
            CT_BARFIGHT,
            CT_BEACHPARTY,
            CT_DEADBODY,
            CT_DROWNING,
            CT_DRUGBUST,
            CT_DUI,
            CT_GANGACTIVITY,
            CT_GRAFFITIARTIST,
            CT_INDECENTEXPOSURE,
            CT_KIDNAPPING,
            CT_LITTERING,
            CT_LOITERING,
            CT_MVA,
            CT_OFFICERDOWN,
            CT_OVERDOSE,
            CT_PAPARAZZI,
            CT_PARKINGVIOLATION,
            CT_PARTY,
            CT_PEDHITBYVEHICLE,
            CT_PEDMISSING,
            CT_PEDONFIRE,
            CT_PEDWITHWEAPON,
            CT_PRANKCALL,
            CT_PURSUIT,
            CT_RECKLESSDRIVING,
            CT_ROBBERY,
            CT_SEARCHWARRANT,
            CT_SEXOFFENDER,
            CT_SPEEDINGVEHICLE,
            CT_STREETPERFORMERFIGHT,
            CT_SUSPICIOUSITEM,
            CT_TRESPASSING,
            CT_VANDALISM,
            CT_VEHICLEFIRE,

            //CT_FORGERY,
            //CT_ILLEGALDEAL,
            //CT_IMPORTING,
            //CT_TERRORISTATTACK,
            //CT_TERRORISTPLOT,
            //CT_UNIONDEPOSITORY,

            //CT_PRISONBREAK,
            //CT_PRISONRIOT,

            //CT_ANIMALVSVEHICLE,
            //CT_ILLEGALHUNTING,
            //CT_OVERKILLLIMIT,
            //CT_POACHING
        }

        /// <summary>
        /// Credits https://stackoverflow.com/questions/3132126/how-do-i-select-a-random-value-from-an-enumeration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomEnumValue<T>()
        {
            Array v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }
    }
}
