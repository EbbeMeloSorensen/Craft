using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Craft.Math;
using Craft.Utils;
using Craft.ViewModels.Common;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Geometry2D.Scrolling;
using PointViewModel = Craft.ViewModels.Common.PointViewModel;

namespace Craft.Algorithms.GuiTest.Tab2
{
    // General Geometry Manipulation
    public class Tab2ViewModel : ImageEditorViewModel
    {
        private List<PointD> _points;
        private ObservableCollection<LineViewModel> _lines;
        private ObservableCollection<PointViewModel> _pointViewModels;
        private bool _pointWasClicked;
        private PointViewModel _activeViewModel;
        private int _indexOfActivePoint;
        private PointD _initialPoint; // At start of drag
        private Brush _brush1;
        private Brush _brush2;
        private Brush _brush3;
        private Brush _brush4;
        private Brush _brush5;
        private bool _showHeights;
        private bool _showMidPointNormals;
        private bool _showMedians;
        private bool _showHalfAngleLines;

        public bool ShowMidPointNormals
        {
            get => _showMidPointNormals;
            set
            {
                _showMidPointNormals = value;
                UpdateLines();
                RaisePropertyChanged();
            }
        }

        public bool ShowHeights
        {
            get => _showHeights;
            set
            {
                _showHeights = value;
                UpdateLines();
                RaisePropertyChanged();
            }
        }

        public bool ShowMedians
        {
            get => _showMedians;
            set
            {
                _showMedians = value;
                UpdateLines();
                RaisePropertyChanged();
            }
        }

        public bool ShowHalfAngleLines
        {
            get => _showHalfAngleLines;
            set
            {
                _showHalfAngleLines = value;
                UpdateLines();
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<LineViewModel> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PointViewModel> PointViewModels
        {
            get
            {
                return _pointViewModels;
            }
            set
            {
                _pointViewModels = value;
                RaisePropertyChanged();
            }
        }

        // We use this to communicate to the view if a point was clicked when a mouse down event occured
        public bool PointWasClicked
        {
            get
            {
                var result = _pointWasClicked;
                _pointWasClicked = false;
                return result;
            }
        }

        public Tab2ViewModel(
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            _brush1 = new SolidColorBrush(Colors.Black);
            _brush2 = new SolidColorBrush(Colors.ForestGreen);
            _brush3 = new SolidColorBrush(Colors.Orange);
            _brush4 = new SolidColorBrush(Colors.DeepSkyBlue);
            _brush5 = new SolidColorBrush(Colors.Magenta);

            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            _points = new List<PointD>
            {
                new PointD(300, 100),
                new PointD(350, 120),
                new PointD(250, 150)
            };

            var pointDiameters = new[] {15, 15, 15};

            _pointViewModels = new ObservableCollection<PointViewModel>();

            var pointIndex = 0;

            _points.ForEach(p =>
            {
                var pointViewModel = new PointViewModel(p, pointIndex, pointDiameters[pointIndex]);
                pointIndex++;

                pointViewModel.ElementClicked += ElementViewModelElementClicked;

                _pointViewModels.Add(pointViewModel);
            });

            UpdateLines();
        }

        public void MovePoint(PointD offset)
        {
            var point = new PointD(
                _initialPoint.X + offset.X,
                _initialPoint.Y + offset.Y);

            _activeViewModel.Point = point;

            _points[_indexOfActivePoint] = new PointD(point.X, point.Y); 

            UpdateLines();
        }

        private void ElementViewModelElementClicked(
            object sender, 
            ElementClickedEventArgs e)
        {
            _pointWasClicked = true;
            _indexOfActivePoint = e.ElementId;
            _activeViewModel = PointViewModels[_indexOfActivePoint];
            _initialPoint = _activeViewModel.Point;
        }

        private void UpdateLines()
        {
            var hLength = 10000;

            var p1 = _points[0];
            var p2 = _points[1];
            var p3 = _points[2];

            var v12 = new Vector2D(p2.X - p1.X, p2.Y - p1.Y);
            var v13 = new Vector2D(p3.X - p1.X, p3.Y - p1.Y);
            var v23 = new Vector2D(p3.X - p2.X, p3.Y - p2.Y);

            var v12_hat = v12.Hat().NormalizeOrSetToZero();
            var v13_hat = v13.Hat().NormalizeOrSetToZero();
            var v23_hat = v23.Hat().NormalizeOrSetToZero();

            var m12 = new Vector2D(
                p1.X + v12.X / 2, 
                p1.Y + v12.Y / 2);

            var m13 = new Vector2D(
                p1.X + v13.X / 2,
                p1.Y + v13.Y / 2);

            var m23 = new Vector2D(
                p2.X + v23.X / 2,
                p2.Y + v23.Y / 2);

            // Højder
            var h1a = new PointD(
                p1.X + v23_hat.X * hLength / 2,
                p1.Y + v23_hat.Y * hLength / 2);

            var h1b = new PointD(
                p1.X - v23_hat.X * hLength / 2,
                p1.Y - v23_hat.Y * hLength / 2);

            var h2a = new PointD(
                p2.X + v13_hat.X * hLength / 2,
                p2.Y + v13_hat.Y * hLength / 2);

            var h2b = new PointD(
                p2.X - v13_hat.X * hLength / 2,
                p2.Y - v13_hat.Y * hLength / 2);

            var h3a = new PointD(
                p3.X + v12_hat.X * hLength / 2,
                p3.Y + v12_hat.Y * hLength / 2);

            var h3b = new PointD(
                p3.X - v12_hat.X * hLength / 2,
                p3.Y - v12_hat.Y * hLength / 2);

            // MidtNormaler
            var m12a = new PointD(
                m12.X + v12_hat.X * hLength / 2,
                m12.Y + v12_hat.Y * hLength / 2);

            var m12b = new PointD(
                m12.X - v12_hat.X * hLength / 2,
                m12.Y - v12_hat.Y * hLength / 2);

            var m13a = new PointD(
                m13.X + v13_hat.X * hLength / 2,
                m13.Y + v13_hat.Y * hLength / 2);

            var m13b = new PointD(
                m13.X - v13_hat.X * hLength / 2,
                m13.Y - v13_hat.Y * hLength / 2);

            var m23a = new PointD(
                m23.X + v23_hat.X * hLength / 2,
                m23.Y + v23_hat.Y * hLength / 2);

            var m23b = new PointD(
                m23.X - v23_hat.X * hLength / 2,
                m23.Y - v23_hat.Y * hLength / 2);

            // Medianer
            var v1 = new Vector2D(
                m23.X - p1.X,
                m23.Y - p1.Y
            ).NormalizeOrSetToZero();

            var v2 = new Vector2D(
                m13.X - p2.X,
                m13.Y - p2.Y
            ).NormalizeOrSetToZero();

            var v3 = new Vector2D(
                m12.X - p3.X,
                m12.Y - p3.Y
            ).NormalizeOrSetToZero();

            var median1a = new PointD(
                p1.X + v1.X * hLength / 2,
                p1.Y + v1.Y * hLength / 2);

            var median1b = new PointD(
                p1.X - v1.X * hLength / 2,
                p1.Y - v1.Y * hLength / 2);

            var median2a = new PointD(
                p2.X + v2.X * hLength / 2,
                p2.Y + v2.Y * hLength / 2);

            var median2b = new PointD(
                p2.X - v2.X * hLength / 2,
                p2.Y - v2.Y * hLength / 2);

            var median3a = new PointD(
                p3.X + v3.X * hLength / 2,
                p3.Y + v3.Y * hLength / 2);

            var median3b = new PointD(
                p3.X - v3.X * hLength / 2,
                p3.Y - v3.Y * hLength / 2);

            // Vinkelhalveringslinjer
            var vec1 = (v12.NormalizeOrSetToZero() + v13.NormalizeOrSetToZero()).NormalizeOrSetToZero();
            var vec2 = (v23.NormalizeOrSetToZero() - v12.NormalizeOrSetToZero()).NormalizeOrSetToZero();
            var vec3 = (v13.NormalizeOrSetToZero() + v23.NormalizeOrSetToZero()).NormalizeOrSetToZero();

            var vh1a = new PointD(
                p1.X + vec1.X * hLength / 2,
                p1.Y + vec1.Y * hLength / 2);

            var vh1b = new PointD(
                p1.X - vec1.X * hLength / 2,
                p1.Y - vec1.Y * hLength / 2);

            var vh2a = new PointD(
                p2.X + vec2.X * hLength / 2,
                p2.Y + vec2.Y * hLength / 2);

            var vh2b = new PointD(
                p2.X - vec2.X * hLength / 2,
                p2.Y - vec2.Y * hLength / 2);

            var vh3a = new PointD(
                p3.X + vec3.X * hLength / 2,
                p3.Y + vec3.Y * hLength / 2);

            var vh3b = new PointD(
                p3.X - vec3.X * hLength / 2,
                p3.Y - vec3.Y * hLength / 2);

            Lines = new ObservableCollection<LineViewModel>
            {
                new LineViewModel(p1, p2, 1, _brush1),
                new LineViewModel(p2, p3, 1, _brush1),
                new LineViewModel(p3, p1, 1, _brush1)
            };

            if (ShowHeights)
            {
                Lines.Add(new LineViewModel(h1a, h1b, 1, _brush2));
                Lines.Add(new LineViewModel(h2a, h2b, 1, _brush2));
                Lines.Add(new LineViewModel(h3a, h3b, 1, _brush2));
            }

            if (ShowMidPointNormals)
            {
                Lines.Add(new LineViewModel(m12a, m12b, 1, _brush3));
                Lines.Add(new LineViewModel(m13a, m13b, 1, _brush3));
                Lines.Add(new LineViewModel(m23a, m23b, 1, _brush3));
            }

            if (ShowMedians)
            {
                Lines.Add(new LineViewModel(median1a, median1b, 1, _brush4));
                Lines.Add(new LineViewModel(median2a, median2b, 1, _brush4));
                Lines.Add(new LineViewModel(median3a, median3b, 1, _brush4));
            }

            if (ShowHalfAngleLines)
            {
                Lines.Add(new LineViewModel(vh1a, vh1b, 1, _brush5));
                Lines.Add(new LineViewModel(vh2a, vh2b, 1, _brush5));
                Lines.Add(new LineViewModel(vh3a, vh3b, 1, _brush5));
            }
        }
    }
}
