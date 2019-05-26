using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResponseVLib
{
    public static class Extension
    {
        public static float DistanceTo(this Rage.Vector3 start, Rage.Vector3 end)
        {
            return (end - start).Length();
        }

        /// <summary>
        /// Gets a random position inside the defined radius
        /// </summary>
        /// <param name="v3"></param>
        /// <param name="radius">Radius</param>
        /// <returns>a random position inside the defined radius</returns>
        public static Rage.Vector3 AroundPosition(this Rage.Vector3 v3, float radius)
        {
            return v3 + new Rage.Vector3(Rage.MathHelper.GetRandomSingle(-radius, radius), Rage.MathHelper.GetRandomSingle(-radius, radius), 0.0f);
        }
    }
}
