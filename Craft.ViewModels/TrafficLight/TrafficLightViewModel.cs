using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.TrafficLight
{
    public class TrafficLightViewModel : ViewModelBase
    {
        private int _diameter;
        private Brush _brush;

        public int Diameter
        {
            get { return _diameter; }
            set
            {
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        public Brush Brush
        {
            get { return _brush; }
            set
            {
                _brush = value;
                RaisePropertyChanged();
            }
        }

        public TrafficLightViewModel(
            int diameter)
        {
            _diameter = diameter;
            Brush = new SolidColorBrush(Colors.DarkGray);
        }
    }
}
