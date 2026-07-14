using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates.Interfaces;

namespace Craft.Simulation.BodyStates;

public class BodyStateDoor : BodyState
{
    public double PercentageOpen { get; set; }

    protected BodyStateDoor(
        Body body) : base(body)
    {
    }

    public BodyStateDoor(
        Body body,
        double percentageOpen) : base(body)
    {
        Position = new Vector2D(0, 0); // Not used
        NaturalVelocity = new Vector2D(0, 0); // Not used

        PercentageOpen = percentageOpen;
    }

    public override BodyState Clone()
    {
        return new BodyStateDoor(Body, PercentageOpen)
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
            PercentageOpen = PercentageOpen
        };
    }
}
