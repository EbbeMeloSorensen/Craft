using System.Linq;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using Craft.ViewModels.Common;
using Craft.Utils;
using Craft.DataStructures.Graph;

namespace Craft.Algorithms.GuiTest.Tab9
{
    // Dijkstra Shortest Path in a hex mesh
    public class Tab9ViewModel : ViewModelBase
    {
        private HashSet<int> _sourceIndexes;
        private HashSet<int> _forbiddenIndexes;
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

        public Tab9ViewModel()
        {
            _sourceIndexes = new HashSet<int>();
            _forbiddenIndexes = new HashSet<int>();

            InitializePixels();
        }

        private void PixelLeftClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _sourceIndexes.Add(e.ElementId);

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

            UpdatePixels();
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
                .Select(i => new PixelViewModel(i, _sourceIndexes.Contains(i)
                    ? new Pixel(255, 255, 255, 255, $"{i}")
                    : new Pixel(50, 50, 50, 255, $"{i}")))
                .ToList();

            PixelViewModels2 = range2
                .Select(i => new PixelViewModel(i, _sourceIndexes.Contains(i)
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

        private void UpdatePixels()
        {
            if (!_sourceIndexes.Any())
            {
                InitializePixels();
                return;
            }

            var graph = new GraphHexMesh(Rows * 2, Cols);

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

            var temp = distances.Select((d, i) => new { Index = i, Distance = d });

            var a = temp.Where(_ => _.Index / Cols % 2 == 0).ToList();
            var b = temp.Where(_ => _.Index / Cols % 2 != 0).ToList();

            PixelViewModels1 = a
                .Select(_ => new PixelViewModel(_.Index, new Pixel(
                    _.Distance < 999999 ? (byte)System.Math.Round(255 * _.Distance / max) : (byte)127,
                    _.Distance < 999999 ? (byte)System.Math.Round(255 * _.Distance / max) : _forbiddenIndexes.Contains(_.Index) ? (byte)0 : (byte)127,
                    _.Distance < 999999 ? (byte)System.Math.Round(255 * _.Distance / max) : (byte)0,
                    255,
                    _.Distance < 999999 ? $"{_.Distance:F2}" : null)))
                .ToList();

            PixelViewModels2 = b
                .Select(_ => new PixelViewModel(_.Index, new Pixel(
                    _.Distance < 999999 ? (byte)System.Math.Round(255 * _.Distance / max) : (byte)127,
                    _.Distance < 999999 ? (byte)System.Math.Round(255 * _.Distance / max) : _forbiddenIndexes.Contains(_.Index) ? (byte)0 : (byte)127,
                    _.Distance < 999999 ? (byte)System.Math.Round(255 * _.Distance / max) : (byte)0,
                    255,
                    _.Distance < 999999 ? $"{_.Distance:F2}" : null)))
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
