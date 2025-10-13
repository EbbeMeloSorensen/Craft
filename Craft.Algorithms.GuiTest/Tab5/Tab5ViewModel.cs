using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.ViewModels.Common;

namespace Craft.Algorithms.GuiTest.Tab5
{
    // General Pixel Image Manipulation
    public class Tab5ViewModel : ViewModelBase
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

        public Tab5ViewModel()
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
            PixelViewModels = Enumerable.Range(0, Rows * Cols)
                .Select(i => new PixelViewModel(i, _pixelIndexes.Contains(i) ? new Pixel(255, 255, 255, 255) : new Pixel(50, 50, 50, 255)))
                .ToList();

            PixelViewModels.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }
    }
}
