
using Craft.Math;

namespace Craft.Algorithms.GuiTest.Tab2
{
    public static class Helpers
    {
        public static Vector2D NormalizeOrSetToZero(this Vector2D vector)
        {
            return vector.SqrLength < 0.00000001 ? new Vector2D(0, 0) : vector.Normalize();
        }
    }
}
