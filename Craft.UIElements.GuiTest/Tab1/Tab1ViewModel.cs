using GalaSoft.MvvmLight;
using Craft.ViewModels.TrafficLight;

namespace Craft.UIElements.GuiTest.Tab1
{
    public class Tab1ViewModel : ViewModelBase
    {
        private string _greeting = "Bamse";

        public string Greeting
        {
            get { return _greeting; }
            set
            {
                _greeting = value;
                RaisePropertyChanged();
            }
        }

        public TrafficLightViewModel TrafficLightViewModel { get; private set; }

        public Tab1ViewModel()
        {
            TrafficLightViewModel = new TrafficLightViewModel(100);
        }
    }
}
