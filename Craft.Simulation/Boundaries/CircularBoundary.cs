using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries.Interfaces;

namespace Craft.Simulation.Boundaries;

public class CircularBoundary : IBoundary
{
    public bool Visible { get; }

    public string Tag { get; }

    public Vector2D Center { get; }

    public double Radius { get; }

    public CircularBoundary(
        Vector2D center,
        double radius,
        string tag = null)
    {
        Center = center;
        Radius = radius;
        Tag = tag;
        Visible = true;
    }

    public double DistanceToPoint(
        Vector2D point)
    {
        var dx = Center.X - point.X;
        var dy = Center.Y - point.Y;

        if (dx != 0 || dy != 0)
        {
            return System.Math.Sqrt(dx * dx + dy * dy) - Radius;
        }

        return 0.0;
    }

    public double DistanceToBody(
        BodyState bodyState)
    {
        switch (bodyState.Body)
        {
            case CircularBody body:
            {
                return DistanceToPoint(bodyState.Position) - body.Radius;
            }
            case RectangularBody body:
            {
                throw new NotImplementedException();
            }
            default:
                throw new ArgumentException();
        }
    }

    public bool Intersects(BodyState bodyState)
    {
        switch (bodyState.Body)
        {
            case CircularBody body:
            {
                return !(DistanceToBody(bodyState) > 0.0);
            }
            case RectangularBody body:
            {
                throw new NotImplementedException();
            }
            default:
                throw new ArgumentException();
        }
    }
}