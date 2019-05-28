using Rage;
using Rage.Native;

namespace ResponseVLib
{
    public static class Native
    {
        public static bool GetSafeCoordForPed(Vector3 Point, out Vector3 outPos)
        {
            return NativeFunction.Natives.GET_SAFE_COORD_FOR_PED<bool>(Point.X, Point.Y, Point.Z, true, out outPos, 16);
        }

        public static bool IsCopNear(Vector3 pos, float radius)
        {
            return NativeFunction.Natives.IS_COP_PED_IN_AREA_3D<bool>(pos.X - radius, pos.Y - radius, pos.Z - radius, pos.X + radius, pos.Y + radius, pos.Z + radius);
        }

        public static void RegisterHatedTargetsAroundPed(this Rage.Ped ped, float radius)
        {
            Rage.Native.NativeFunction.Natives.REGISTER_HATED_TARGETS_AROUND_PED(ped, radius);
        }

        public static void RemoveAllWeapons(this Rage.Ped ped)
        {
            Rage.Native.NativeFunction.Natives.REMOVE_ALL_PED_WEAPONS(ped, false);
        }
    }
}
