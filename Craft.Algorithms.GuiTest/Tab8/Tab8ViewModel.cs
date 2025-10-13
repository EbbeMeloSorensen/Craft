using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Common;

namespace Craft.Algorithms.GuiTest.Tab8
{
    // Hexagons
    public class Tab8ViewModel : ViewModelBase
    {
        private HashSet<int> _pixelIndexes;
        private List<PixelViewModel> _pixelViewModels1;
        private List<PixelViewModel> _pixelViewModels2;

        public int Rows
        {
            get { return 5; }
        }

        public int Cols
        {
            get { return 16; }
        }

        public List<PixelViewModel> PixelViewModels1
        {
            get { return _pixelViewModels1; }
            set
            {
                _pixelViewModels1 = value;
                RaisePropertyChanged();
            }
        }

        public List<PixelViewModel> PixelViewModels2
        {
            get { return _pixelViewModels2; }
            set
            {
                _pixelViewModels2 = value;
                RaisePropertyChanged();
            }
        }

        public Tab8ViewModel()
        {
            _pixelIndexes = new HashSet<int>();

            InitializePixels();
        }

        private void PixelLeftClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pixelIndexes.Add(e.ElementId);

            InitializePixels();
        }

        private void PixelRightClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            if (_pixelIndexes.Contains(e.ElementId))
            {
                _pixelIndexes.Remove(e.ElementId);
            }

            InitializePixels();
        }


        private void InitializePixels()
        {
            var range1 = Enumerable.Range(0, Rows)
                .Select(i => Enumerable.Range(i * 2 * Cols, Cols))
                .SelectMany(_ => _);

            var range2 = Enumerable.Range(0, Rows)
                .Select(i => Enumerable.Range(i * 2 * Cols + Cols, Cols))
                .SelectMany(_ => _);

            PixelViewModels1 = range1
                .Select(i => new PixelViewModel(i, _pixelIndexes.Contains(i) 
                    ? new Pixel(255, 255, 255, 255, $"{i}") 
                    : new Pixel(50, 50, 50, 255, $"{i}")))
                .ToList();

            PixelViewModels2 = range2
                .Select(i => new PixelViewModel(i, _pixelIndexes.Contains(i)
                    ? new Pixel(255, 255, 255, 255, $"{i}")
                    : new Pixel(50, 50, 50, 255, $"{i}")))
                .ToList();

            PixelViewModels1.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });

            PixelViewModels2.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }
    }
}
