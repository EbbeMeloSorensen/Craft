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
        private object _currentViewModel;
        private RelayCommand _switchViewModelCommand;

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged();
            }
        }

        //public SimulationLaboratoryViewModel SimulationLaboratoryViewModel { get; }

        public RelayCommand SwitchViewModelCommand
        {
            get
            {
                return _switchViewModelCommand ?? (_switchViewModelCommand = new RelayCommand(SwitchViewModel));
            }
        }

        public MainWindowViewModel()
        {
            //SimulationLaboratoryViewModel = new SimulationLaboratoryViewModel();

            CurrentViewModel = new SimulationLaboratoryViewModel();
            //CurrentViewModel = new DummyViewModel();
        }

        private void SwitchViewModel()
        {
            CurrentViewModel = new DummyViewModel();
        }
    }
}
