using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using Craft.DataStructures.Graph;

namespace Craft.DataStructures.UnitTest;

public class StateMachineTest
{
    [Fact]
    public void FunWithStateMachineForTrafficLight()
    {
        // Arrange
        var red = new State("Red");
        var redAndYellow = new State("Red and Yellow");
        var green = new State("Green");
        var yellow = new State("Yellow");

        var trafficLight = new StateMachine(red);
        trafficLight.AddState(redAndYellow);
        trafficLight.AddState(green);
        trafficLight.AddState(yellow);
        trafficLight.AddTransition(red, redAndYellow);
        trafficLight.AddTransition(redAndYellow, green);
        trafficLight.AddTransition(green, yellow);
        trafficLight.AddTransition(yellow, red);

        // Act/Assert
        trafficLight.CurrentState.Name.Should().Be("Red");
        trafficLight.ExitsFromCurrentState().Count().Should().Be(1);

        trafficLight.SwitchState();
        trafficLight.CurrentState.Name.Should().Be("Red and Yellow");

        trafficLight.SwitchState();
        trafficLight.CurrentState.Name.Should().Be("Green");

        trafficLight.SwitchState();
        trafficLight.CurrentState.Name.Should().Be("Yellow");

        trafficLight.SwitchState();
        trafficLight.CurrentState.Name.Should().Be("Red");
    }

    [Fact]
    public void FunWithStateMachineForLockableDoor()
    {
        // Arrange
        var closed = new State("Closed");
        var open = new State("Open");
        var locked = new State("Locked");

        var lockableDoor = new StateMachine(closed);
        lockableDoor.AddState(open);
        lockableDoor.AddState(locked);
        lockableDoor.AddTransition(closed, open);
        lockableDoor.AddTransition(closed, locked);
        lockableDoor.AddTransition(open, closed);
        lockableDoor.AddTransition(locked, closed);

        // Act/Assert
        lockableDoor.CurrentState.Name.Should().Be("Closed");
        lockableDoor.ExitsFromCurrentState().Count().Should().Be(2);

        lockableDoor.SwitchState("Open");
        lockableDoor.CurrentState.Name.Should().Be("Open");
        lockableDoor.ExitsFromCurrentState().Count().Should().Be(1);

        lockableDoor.SwitchState("Closed");
        lockableDoor.CurrentState.Name.Should().Be("Closed");
        lockableDoor.ExitsFromCurrentState().Count().Should().Be(2);

        lockableDoor.SwitchState("Locked");
        lockableDoor.CurrentState.Name.Should().Be("Locked");
        lockableDoor.ExitsFromCurrentState().Count().Should().Be(1);

        // Notice taht we dont provide the name of the exit here, which is legitimate, because there is only one exit
        lockableDoor.SwitchState();
        lockableDoor.CurrentState.Name.Should().Be("Closed");
        lockableDoor.ExitsFromCurrentState().Count().Should().Be(2);
    }

    [Fact]
    public void FunWithStateMachineForLockableDoor_ThrowsWhenCallingSwitchStateWithoutTransitionNameWhenThereAreMultipleExits()
    {
        // Arrange
        var closed = new State("Closed");
        var open = new State("Open");
        var locked = new State("Locked");

        var lockableDoor = new StateMachine(closed);
        lockableDoor.AddState(open);
        lockableDoor.AddState(locked);
        lockableDoor.AddTransition(closed, open);
        lockableDoor.AddTransition(closed, locked);
        lockableDoor.AddTransition(open, closed);
        lockableDoor.AddTransition(locked, closed);

        // Act/Assert
        lockableDoor.CurrentState.Name.Should().Be("Closed");
        lockableDoor.ExitsFromCurrentState().Count().Should().Be(2);

        // Should throw because there are two exits from the state
        Assert.Throws<InvalidOperationException>(() => lockableDoor.SwitchState());
    }
}