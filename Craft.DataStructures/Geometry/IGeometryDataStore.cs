namespace Craft.DataStructures.Geometry;

public interface IGeometryDataStore : IGeometryDataSource
{
    void AddGeometricObject(
        object geometricObject,
        BoundingBox boundingBox);
}