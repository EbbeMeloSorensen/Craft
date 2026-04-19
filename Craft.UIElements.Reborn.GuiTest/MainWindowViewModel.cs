using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Craft.DataStructures.Geometry;
using Craft.ViewModels.Geometry2D.Reborn;

namespace Craft.UIElements.Reborn.GuiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ICommand SetWorldWindowCommand { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            GeometryViewModel = new GeometryViewModel();

            SetWorldWindowCommand = new RelayCommand(SetWorldWindow);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void SetWorldWindow()
        {
            GeometryViewModel.RequestedWorldWindow = new BoundingBox(-500, 200, -500, 200);
        }
    }
}
