using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Common;

namespace Craft.Algorithms.GuiTest.Tab7
{
    // Dilation
    public class Tab7ViewModel : ViewModelBase
    {
        private HashSet<int> _pixelIndexes;
        private List<PixelViewModel> _pixelViewModels;
        private double _dilation;

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

        public double Dilation
        {
            get { return _dilation; }
            set
            {
                _dilation = value;
                RaisePropertyChanged();

                InitializePixes();
            }
        }

        public Tab7ViewModel()
        {
            _pixelIndexes = new HashSet<int>();
            _dilation = 2;

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

            // Dilate the image
            image.Dilate(Dilation);

            PixelViewModels = image
                .Cast<int>()
                .Select((v, i) => new PixelViewModel(i, _pixelIndexes.Contains(i)
                    ? new Pixel(127, 0, 0, 255)
                    : new Pixel(
                        (byte)System.Math.Round(v * 63.0),
                        0,
                        0,
                        255)))
                .ToList();

            PixelViewModels.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }
    }
}
