using System.Collections.Generic;

namespace Craft.DataStructures.IO.gml
{
    public class MultiSurface : AbstractGeometricPrimitive
    {
        public List<SurfaceMember> SurfaceMembers { get; set; }
    }
}
