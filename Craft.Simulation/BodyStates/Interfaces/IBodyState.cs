using Craft.Simulation.Bodies;

namespace Craft.Simulation.BodyStates.Interfaces;

// Essentielt set har en bodystate ikke nødvendigvis en position eller en hastighed,
// så dette bør være det objekt, som State og Engine baserer sig på.
// Måske skal det endda laves sådan at en state kan pege på en fælles basisklasse for
// Body og Boundary
// Det er en relativt stor refaktorering, som kræver et bedre setup end du har her i
// Brasilien, så skyd refaktoreringen til hjørne i første omgang og løs dit dær design 
// med en specialisering af bodystate, der bare ignorerer de sædvanlige properties
public interface IBodyState
{
    Body Body { get; }

    BodyState Clone();
}
