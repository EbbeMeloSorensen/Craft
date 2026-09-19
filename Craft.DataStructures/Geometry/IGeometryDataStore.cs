namespace Craft.DataStructures.Geometry;

public interface IGeometryDataStore : IGeometryDataSource
{
    void AddGeometricObject(
        object geometricObject,
        BoundingBox boundingBox);

    void RemoveGeometricObjects(
        IEnumerable<object> geometricObjects);
}