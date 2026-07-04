using Craft.DataStructures.Geometry;
using Craft.Logging;
using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Simulation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Point = System.Windows.Point;

namespace Craft.Simulation.Reborn.GuiTest
{
    public class MainWindowViewModel : ViewModelBase
    {
        public SimulationLaboratoryViewModel SimulationLaboratoryViewModel { get; }

        public MainWindowViewModel()
        {
            SimulationLaboratoryViewModel = new SimulationLaboratoryViewModel();
        }
    }
}
