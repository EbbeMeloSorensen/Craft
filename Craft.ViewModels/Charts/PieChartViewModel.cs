using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace Craft.ViewModels.Charts
{
    public class PieChartViewModel : ViewModelBase
    {
        private List<Brush> _palette = new List<Brush>
        {
            new SolidColorBrush(Colors.DodgerBlue),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.Cyan),
            new SolidColorBrush(Colors.Gray),
            new SolidColorBrush(Colors.Yellow),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.BlueViolet),
            new SolidColorBrush(Colors.SaddleBrown),
            new SolidColorBrush(Colors.LightCoral),
            new SolidColorBrush(Colors.Magenta),
            new SolidColorBrush(Colors.LightGreen),
            new SolidColorBrush(Colors.DarkCyan),
            new SolidColorBrush(Colors.MediumVioletRed),
            new SolidColorBrush(Colors.DarkSlateBlue),
            new SolidColorBrush(Colors.DarkRed)
        };

        private string _title;
        private double _width;
        private double _height;
        private bool _singlePieSliceInChart;
        private string _firstTag;
        private Point _centerPoint;
        private Brush _paletteColorForSinglePieSlice;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged();
            }
        }

        public string FirstTag
        {
            get { return _firstTag; }
            set
            {
                _firstTag = value;
                RaisePropertyChanged();
            }
        }

        public Point CenterPoint
        {
            get { return _centerPoint; }
            set
            {
                _centerPoint = value;
                RaisePropertyChanged();
            }
        }

        public bool SinglePieSliceInChart
        {
            get { return _singlePieSliceInChart; }
            set
            {
                _singlePieSliceInChart = value;
                RaisePropertyChanged();
            }
        }

        public Brush PaletteColorForSinglePieSlice
        {
            get { return _paletteColorForSinglePieSlice; }
            set
            {
                _paletteColorForSinglePieSlice = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PieSliceViewModel> PieSliceViewModels { get; private set; }

        public ObservableCollection<PieSliceDescriptionViewModel> PieSliceDescriptionViewModels { get; private set; }

        public PieChartViewModel()
        {
            PieSliceViewModels = new ObservableCollection<PieSliceViewModel>();
            PieSliceDescriptionViewModels = new ObservableCollection<PieSliceDescriptionViewModel>();
        }

        public void Initialize(
            string title,
            double diameter,
            Dictionary<string, double> distribution)
        {
            Title = title;
            Width = diameter * 1.5;
            Height = diameter;

            var sum = distribution.Sum(kvp => kvp.Value);
            var angleRotation = 0.0;
            var index = 0;

            PieSliceViewModels.Clear();
            PieSliceDescriptionViewModels.Clear();

            // Legend-striben må ikke være højere end diameteren
            var width = 0.075 * diameter;
            var preferredHeight = 0.075 * diameter;
            var preferredSpacing = preferredHeight * 0.1;

            var heightOfDescriptionListForPreferredHeight =
                preferredHeight * distribution.Count + preferredSpacing * (distribution.Count - 1);

            var height = heightOfDescriptionListForPreferredHeight <= diameter
                ? preferredHeight
                : diameter / (1.1 * distribution.Count - 0.1);

            foreach (var kvp in distribution)
            {
                var brush = _palette[index % _palette.Count];

                var angleSize = 360 * kvp.Value / sum;
                var tag = kvp.Value.ToString(CultureInfo.InvariantCulture);
                PieSliceViewModels.Add(new PieSliceViewModel(diameter, angleSize, angleRotation, brush, tag));
                angleRotation += angleSize;

                var description = kvp.Key;

                PieSliceDescriptionViewModels.Add(new PieSliceDescriptionViewModel(
                    description, width, height, diameter * 1.1, 1.1 * index * height, brush));

                index++;
            }

            SinglePieSliceInChart = distribution.Count(kvp => kvp.Value > 0.0) == 1;

            if (SinglePieSliceInChart)
            {
                FirstTag = distribution.Single(kvp => kvp.Value > 0.0).Value.ToString(CultureInfo.InvariantCulture);
                CenterPoint = new Point(diameter / 2, diameter / 2);

                // Determine the color of the single circular pie slice
                index = 0;
                foreach (var kvp in distribution)
                {
                    if (kvp.Value > 0.0)
                    {
                        PaletteColorForSinglePieSlice = _palette[index % _palette.Count];
                        break;
                    }

                    index++;
                }
            }
        }

        public void OverrideDefaultPalette(
            List<Brush> customPalette)
        {
            _palette = customPalette;
        }
    }
}
