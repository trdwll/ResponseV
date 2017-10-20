using System;

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

        public static bool GetRandBool()
        {
            return RANDOM.Next(2) == 0;
        }

        public static double GetRandDouble()
        {
            return RANDOM.NextDouble();
        }
    }
}
