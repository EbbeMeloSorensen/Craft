using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Common;

namespace Craft.Algorithms.GuiTest.Tab6
{
    // Euclidean Distance Transform
    public class Tab6ViewModel : ViewModelBase
    {
        private HashSet<int> _pixelIndexes;
        private List<PixelViewModel> _pixelViewModels;

        public int Rows
        {
            get { return 10; }
        }

        public int Cols
        {
            get { return 16; }
        }

        public List<PixelViewModel> PixelViewModels
        {
            get { return _pixelViewModels; }
            set
            {
                _pixelViewModels = value;
                RaisePropertyChanged();
            }
        }

        public Tab6ViewModel()
        {
            _pixelIndexes = new HashSet<int>();

            InitializePixes();
        }

        private void PixelLeftClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pixelIndexes.Add(e.ElementId);

            InitializePixes();
        }

        private void PixelRightClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            if (_pixelIndexes.Contains(e.ElementId))
            {
                _pixelIndexes.Remove(e.ElementId);
            }

            InitializePixes();
        }

        private void InitializePixes()
        {
            if (!_pixelIndexes.Any())
            {
                PixelViewModels = Enumerable.Range(0, Rows * Cols)
                    .Select(i => new PixelViewModel(i, new Pixel(50, 50, 50, 255)))
                    .ToList();

                PixelViewModels.ForEach(p =>
                {
                    p.PixelLeftClicked += PixelLeftClicked;
                    p.PixelRightClicked += PixelRightClicked;
                });

                return;
            }

            var image = new int[Rows, Cols];

            _pixelIndexes.ToList().ForEach(i =>
            {
                var x = i % Cols;
                var y = i / Cols;
                image[y, x] = 1;
            });

            DistanceTransform.EuclideanDistanceTransform(image, out var distances, out var xValues, out var yValues);

            var max = distances.Cast<double>().Max();

            // Bemærk lige cast metoden til at konvertere 2d arrayet til Enumerable af double
            PixelViewModels = distances.Cast<double>()
                .Select((d, i) => new PixelViewModel(i, _pixelIndexes.Contains(i) 
                    ? new Pixel(127, 0, 0, 255) 
                    : new Pixel(
                        (byte)System.Math.Round(255 * d / max), 
                        (byte)System.Math.Round(255 * d / max), 
                        (byte)System.Math.Round(255 * d / max), 
                        255,
                        $"{d:F2}")))
                .ToList();

            PixelViewModels.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }
    }
}
