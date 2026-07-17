using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates.Interfaces;

namespace Craft.Simulation.BodyStates;

public class BodyStateDoor : BodyState
{
    public bool OpenClockWise { get; set; }
    public double PercentageOpen { get; set; }

    protected BodyStateDoor(
        Body body) : base(body)
    {
    }

    public BodyStateDoor(
        Body body,
        bool openClockWise,
        double percentageOpen) : base(body)
    {
        Position = new Vector2D(0, 0); // Not used
        NaturalVelocity = new Vector2D(0, 0); // Not used

        OpenClockWise = openClockWise;
        PercentageOpen = percentageOpen;
    }

    public void SetOpeningDirection(
        Vector2D playerPosition)
    {
        var door = Body as BodyDoor;

        var p = playerPosition.AsPoint2D();
        var l1 = door.Point2.AsPoint2D();
        var l2 = door.Point1.AsPoint2D();

        var sideOfLine = Operations.SideOfLine(p, l1, l2);

        OpenClockWise = sideOfLine == 1;
    }

    public override BodyState Clone()
    {
        return new BodyStateDoor(Body, OpenClockWise, PercentageOpen)
        {
            Position = Position,
            NaturalVelocity = NaturalVelocity,
        };
    }

    public override BodyState Propagate(
        double time,
        Vector2D force)
    {
        return new BodyStateDoor(Body)
        {
            Position = Position,
            NaturalVelocity = NaturalVelocity,
            PercentageOpen = PercentageOpen,
            OpenClockWise = OpenClockWise
        };
    }
}
