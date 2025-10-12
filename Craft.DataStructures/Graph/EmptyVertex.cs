using System.Reflection.Emit;

namespace Craft.DataStructures.Graph
{
    public class EmptyVertex : IVertex
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return null;
        }
    }
}