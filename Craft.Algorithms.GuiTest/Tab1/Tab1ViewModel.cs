using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using Craft.Utils;
using Craft.DataStructures.Graph;
using Craft.ViewModels.Common;

namespace Craft.Algorithms.GuiTest.Tab1
{
    public enum Mode
    {
        Source,
        Forbidden
    }

    // Dijkstra Shortest Path
    public class Tab1ViewModel : ViewModelBase
    {
        private Mode _currentMode = Mode.Source;
        private HashSet<int> _sourceIndexes;
        private HashSet<int> _forbiddenIndexes;
        private List<PixelViewModel> _pixelViewModels;

        public Mode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                _currentMode = value;
                RaisePropertyChanged();
            }
        }

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

        public Tab1ViewModel()
        {
            _sourceIndexes = new HashSet<int>();
            _forbiddenIndexes = new HashSet<int>();

            InitializePixels();
        }

        private void PixelLeftClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            switch (CurrentMode)
            {
                case Mode.Source:
                    _sourceIndexes.Add(e.ElementId);
                    break;
                case Mode.Forbidden:
                    _forbiddenIndexes.Add(e.ElementId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdatePixels();
        }

        private void PixelRightClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            if (_sourceIndexes.Contains(e.ElementId))
            {
                _sourceIndexes.Remove(e.ElementId);
            }
            else if (_forbiddenIndexes.Contains(e.ElementId))
            {
                _forbiddenIndexes.Remove(e.ElementId);
            }

            UpdatePixels();
        }

        private void InitializePixels()
        {
            PixelViewModels = Enumerable.Range(0, Rows * Cols)
                .Select(i => new PixelViewModel(i, _forbiddenIndexes.Contains(i) ? new Pixel(127, 0, 0, 127) : new Pixel(50, 50, 50, 255)))
                .ToList();

            PixelViewModels.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }

        private void UpdatePixels()
        {
            if (!_sourceIndexes.Any())
            {
                InitializePixels();
                return;
            }

            var graph = new GraphMatrix8Connectivity(Rows, Cols);

            graph.ComputeDistances(
                _sourceIndexes,
                _forbiddenIndexes,
                double.MaxValue,
                out var distances,
                out var previous);

            var max = distances.Where(d => d < 999999).Max();

            // Walls are drawn in red
            // Unreachable areas are drawn in yellow
            // Other pixels are drawn in gray

            PixelViewModels = distances
                .Select((d, i) => new PixelViewModel(i, new Pixel(
                    d < 999999 ? (byte)System.Math.Round(255 * d / max) : (byte)127,
                    d < 999999 ? (byte)System.Math.Round(255 * d / max) : _forbiddenIndexes.Contains(i) ? (byte)0 : (byte)127,
                    d < 999999 ? (byte)System.Math.Round(255 * d / max) : (byte)0,
                    255,
                    d < 999999 ? $"{d:F2}" : null)))
                .ToList();

            PixelViewModels.ForEach(p =>
            {
                p.PixelLeftClicked += PixelLeftClicked;
                p.PixelRightClicked += PixelRightClicked;
            });
        }
    }
}
