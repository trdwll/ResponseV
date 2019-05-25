using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseVLib
{
    public static class Utils
    {
        public static T GetRandValue<T>(params T[] args)
        {
            return args[MathHelper.GetRandomInteger(args.Length - 1)];
        }

        public static bool GetRandBool()
        {
            return MathHelper.GetRandomDouble(0.0, 1.0) >= 0.5;
        }

        public static T[] MergeArrays<T>(T[] array1, T[] array2)
        {
            T[] tmp = new T[array1.Length + array2.Length];
            array1.CopyTo(tmp, 0);
            array2.CopyTo(tmp, array1.Length);

            return tmp;
        }
    }
}
