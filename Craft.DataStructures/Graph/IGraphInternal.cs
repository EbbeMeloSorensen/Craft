namespace Craft.DataStructures.Graph;

public interface IGraphInternal : IGraph
{
    void RebuildInternalState();
}