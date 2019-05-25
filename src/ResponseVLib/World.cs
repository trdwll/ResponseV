using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseVLib
{
    public static class World
    {
        public static bool IsNight()
        {
            TimeSpan now = Rage.World.TimeOfDay;
            return now.Hours >= 20 || now.Hours < 6;
        }

        public static bool IsDay()
        {
            return !IsNight();
        }
    }
}
