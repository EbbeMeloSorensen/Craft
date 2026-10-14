using Craft.DataStructures.Geometry;
using Craft.DataStructures.MxCifQuadTree;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;

namespace Craft.Simulation.Engine;

/// <summary>
/// Spatial candidates for circular bodies against doors with fixed hinges and dimensions.
/// Rebuild this index if doors are added, removed, or their endpoints change.
/// Door opening state may change without rebuilding.
/// </summary>
public sealed class DoorCollisionBroadPhase
{
    private readonly DataStore? _doors;

    public DoorCollisionBroadPhase(IEnumerable<BodyDoor> doors)
    {
        var entries = doors.Select(door =>
        {
            // Cover both the closed segment and the engine's rotated endpoint.
            // Its perpendicular component is a unit vector, even for narrow doors.
            var extent = System.Math.Max(1.0, (door.Point2 - door.Point1).Length);
            return (Door: door, Bounds: new BoundingBox(
                door.Point1.X - extent, door.Point1.X + extent,
                door.Point1.Y - extent, door.Point1.Y + extent));
        }).ToArray();

        if (entries.Length == 0)
        {
            return;
        }

        _doors = new DataStore(new BoundingBox(
            entries.Min(e => e.Bounds.MinX), entries.Max(e => e.Bounds.MaxX),
            entries.Min(e => e.Bounds.MinY), entries.Max(e => e.Bounds.MaxY)));
        foreach (var entry in entries)
        {
            _doors.AddGeometricObject(entry.Door, entry.Bounds);
        }
    }

    public IEnumerable<(BodyState First, BodyState Second)> GetCandidates(
        Dictionary<BodyState, BodyState> states)
    {
        if (_doors == null)
        {
            yield break;
        }

        var doorStates = new Dictionary<BodyDoor, BodyState>();
        foreach (var after in states.Keys)
        {
            if (after.Body is BodyDoor door)
            {
                doorStates.Add(door, after);
            }
        }

        foreach (var (after, before) in states)
        {
            if (after.Body is not CircularBody circle)
            {
                continue;
            }

            // Include the whole movement segment and radius, including exact contact.
            var radius = circle.Radius + 1e-12;
            var bounds = new BoundingBox(
                System.Math.Min(before.Position.X, after.Position.X) - radius,
                System.Math.Max(before.Position.X, after.Position.X) + radius,
                System.Math.Min(before.Position.Y, after.Position.Y) - radius,
                System.Math.Max(before.Position.Y, after.Position.Y) + radius);
            foreach (BodyDoor door in _doors.GetIntersecting(bounds))
            {
                if (doorStates.TryGetValue(door, out var doorState))
                {
                    yield return (after, doorState);
                }
            }
        }
    }
}
