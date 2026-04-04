using Craft.ViewModels.TrafficLight;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.Laboratory.GuiTest.Tab1
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
