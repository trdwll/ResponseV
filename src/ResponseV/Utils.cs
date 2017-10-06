using System;

namespace ResponseV
{
    public class Utils
    {
        public static int getRandInt(int min, int max)
        {
            return new Random().Next(min, max);
        }
    }
}
