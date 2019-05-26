using System.Linq;

using Rage;
using Rage.Native;

namespace ResponseVLib
{
    public static class Node
    {
        public static readonly int[] BlackListedNodeTypes = new int[] { 0, 8, 9, 10, 12, 40, 42, 136 };

        public static int GetNearestNodeType(this Vector3 pos)
        {
            uint node_prop_p1;
            int found_node_type;
            bool get_property_success = NativeFunction.Natives.GET_VEHICLE_NODE_PROPERTIES<bool>(pos.X, pos.Y, pos.Z, out node_prop_p1, out found_node_type);

            return get_property_success ? found_node_type : -1;
        }

        public static bool IsNodeSafe(this Vector3 pos)
        {
            return !BlackListedNodeTypes.Contains(GetNearestNodeType(pos));
        }

        public static bool IsPointOnWater(this Vector3 position)
        {
            float height;
            return NativeFunction.Natives.GET_WATER_HEIGHT<bool>(position.X, position.Y, position.Z, out height);
        }

        /// <summary>
        /// Returns the heading of the closest vehicle node
        /// </summary>
        /// <param name="v3"></param>
        /// <returns>the heading of the closest vehicle node</returns>
        public static float GetClosestVehicleNodeHeading(this Vector3 v3)
        {
            float outHeading;
            Vector3 outPosition;

            NativeFunction.Natives.GET_CLOSEST_VEHICLE_NODE_WITH_HEADING(v3.X, v3.Y, v3.Z, out outPosition, out outHeading, 12, 0x40400000, 0);

            return outHeading;
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this ISpatial spatial, ISpatial towards)
        {
            return GetHeadingTowards(spatial, towards.Position);
        }

        /// <summary>
        /// Gets the heading towards a position
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsPosition">Position to face to</param>
        /// <returns>the heading towards a position</returns>
        public static float GetHeadingTowards(this ISpatial spatial, Vector3 towardsPosition)
        {
            return GetHeadingTowards(spatial.Position, towardsPosition);
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, Vector3 towardsPosition)
        {
            Vector3 directionFromEntityToPosition = (towardsPosition - position);
            directionFromEntityToPosition.Normalize();

            float heading = MathHelper.ConvertDirectionToHeading(directionFromEntityToPosition);
            return heading;
        }

        /// <summary>
        /// Gets the heading towards an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="towardsEntity">Entity to face to</param>
        /// <returns>the heading towards an entity</returns>
        public static float GetHeadingTowards(this Vector3 position, ISpatial towards)
        {
            return GetHeadingTowards(position, towards.Position);
        }

    }
}
