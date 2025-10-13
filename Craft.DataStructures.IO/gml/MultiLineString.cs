using System.Collections.Generic;

namespace Craft.DataStructures.IO.gml
{
    public class MultiLineString : AbstractGeometricPrimitive
    {
        public List<LineStringMember> LineStringMembers { get; set; }
    }
}
