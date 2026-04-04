using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Geometry2D.Scrolling;

namespace Craft.UIElements.GuiTest.Tab3
{
    public class Tab3ViewModel : ViewModelBase
    {
        private bool _includeGrid = true;
        private bool _includeTicks = false;
        private Brush _coordinateSystemBrush = new SolidColorBrush(Colors.Gray);
        private Brush _gridBrush = new SolidColorBrush(Colors.Gray) {Opacity = 0.25};
        private Brush _curveBrush = new SolidColorBrush(Colors.Black);

        // Initielt World Window (afgrænset højre og venstre)
        private double _x0 = -3.0;
        private double _x1 = 4.0;
        private double _y0 = -1.0;
        private double _y1 = 1.0;

        private bool _windMillInHouseDrawingsRotates;
        private bool _allowROISelectionForGeometryEditorViewModel1;
        private double _worldWindowLimitLeftForGeometryEditorViewModel1;
        private double _worldWindowLimitRightForGeometryEditorViewModel1;
        private double _worldWindowLimitTopForGeometryEditorViewModel1;
        private double _worldWindowLimitBottomForGeometryEditorViewModel1;
        private int _worldWindowUpdateCountForGeometryEditorViewModel4;
        private int _worldWindowMajorUpdateCountForGeometryEditorViewModel4;
        private int _worldWindowUpdateCountForCoordinateSystemViewModel;
        private int _worldWindowMajorUpdateCountForCoordinateSystemViewModel;
        private string _cursorPositionForGeometryEditorViewModel3AsText;
        private string _cursorPositionForImageEditorViewModelAsText;
        private string _timeAtMousePositionAsText1;
        private string _timeAtMousePositionAsText2;
        private Stopwatch _stopwatch;
        private List<DateTime> _timeStampsOfInterest;
        private string _roiXAsText;
        private string _roiYAsText;

        private RelayCommand _applyWorldWindowLimitsForGeometryEditor1Command;
        private RelayCommand _zoomInForGeometryEditor1Command;
        private RelayCommand _zoomOutForGeometryEditor1Command;
        private RelayCommand _setSelectedRegionForGeometryEditor1Command;
        private RelayCommand _setWorldWindowForGeometryEditor1Command;
        private RelayCommand _zoomInForGeometryEditor2Command;
        private RelayCommand _zoomOutForGeometryEditor2Command;

        public string ROIXAsText
        {
            get => _roiXAsText;
            set
            {
                _roiXAsText = value;
                RaisePropertyChanged();
            }
        }

        public string ROIYAsText
        {
            get => _roiYAsText;
            set
            {
                _roiYAsText = value;
                RaisePropertyChanged();
            }
        }

        public bool WindMillInHouseDrawingsRotates
        {
            get { return _windMillInHouseDrawingsRotates; }
            set
            {
                _windMillInHouseDrawingsRotates = value;
                RaisePropertyChanged();
            }
        }

        public bool AllowROISelectionForGeometryEditorViewModel1
        {
            get { return _allowROISelectionForGeometryEditorViewModel1; }
            set
            {
                _allowROISelectionForGeometryEditorViewModel1 = value;

                if (GeometryEditorViewModel1 != null)
                {
                    GeometryEditorViewModel1.SelectRegionPossible = _allowROISelectionForGeometryEditorViewModel1;
                }

                RaisePropertyChanged();
            }
        }

        public int WorldWindowUpdateCountForGeometryEditorViewModel4
        {
            get { return _worldWindowUpdateCountForGeometryEditorViewModel4; }
            set
            {
                _worldWindowUpdateCountForGeometryEditorViewModel4 = value;
                RaisePropertyChanged();
            }
        }

        public double WorldWindowLimitLeftForGeometryEditorViewModel1
        {
            get { return _worldWindowLimitLeftForGeometryEditorViewModel1; }
            set
            {
                _worldWindowLimitLeftForGeometryEditorViewModel1 = value;
                RaisePropertyChanged();
            }
        }

        public double WorldWindowLimitRightForGeometryEditorViewModel1
        {
            get { return _worldWindowLimitRightForGeometryEditorViewModel1; }
            set
            {
                _worldWindowLimitRightForGeometryEditorViewModel1 = value;
                RaisePropertyChanged();
            }
        }

        public double WorldWindowLimitTopForGeometryEditorViewModel1
        {
            get { return _worldWindowLimitTopForGeometryEditorViewModel1; }
            set
            {
                _worldWindowLimitTopForGeometryEditorViewModel1 = value;
                RaisePropertyChanged();
            }
        }

        public double WorldWindowLimitBottomForGeometryEditorViewModel1
        {
            get { return _worldWindowLimitBottomForGeometryEditorViewModel1; }
            set
            {
                _worldWindowLimitBottomForGeometryEditorViewModel1 = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowMajorUpdateCountForGeometryEditorViewModel4
        {
            get { return _worldWindowMajorUpdateCountForGeometryEditorViewModel4; }
            set
            {
                _worldWindowMajorUpdateCountForGeometryEditorViewModel4 = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowUpdateCountForCoordinateSystemViewModel
        {
            get { return _worldWindowUpdateCountForCoordinateSystemViewModel; }
            set
            {
                _worldWindowUpdateCountForCoordinateSystemViewModel = value;
                RaisePropertyChanged();
            }
        }

        public int WorldWindowMajorUpdateCountForCoordinateSystemViewModel
        {
            get { return _worldWindowMajorUpdateCountForCoordinateSystemViewModel; }
            set
            {
                _worldWindowMajorUpdateCountForCoordinateSystemViewModel = value;
                RaisePropertyChanged();
            }
        }

        public string CursorPositionForGeometryEditorViewModel3AsText
        {
            get { return _cursorPositionForGeometryEditorViewModel3AsText; }
            set
            {
                _cursorPositionForGeometryEditorViewModel3AsText = value;
                RaisePropertyChanged();
            }
        }

        public string CursorPositionForImageEditorViewModelAsText
        {
            get { return _cursorPositionForImageEditorViewModelAsText; }
            set
            {
                _cursorPositionForImageEditorViewModelAsText = value;
                RaisePropertyChanged();
            }
        }

        public string TimeAtMousePositionAsText1
        {
            get { return _timeAtMousePositionAsText1; }
            set
            {
                _timeAtMousePositionAsText1 = value;
                RaisePropertyChanged();
            }
        }

        public string TimeAtMousePositionAsText2
        {
            get { return _timeAtMousePositionAsText2; }
            set
            {
                _timeAtMousePositionAsText2 = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ApplyWorldWindowLimitsForGeometryEditor1Command
        {
            get
            {
                return _applyWorldWindowLimitsForGeometryEditor1Command ?? (_applyWorldWindowLimitsForGeometryEditor1Command = new RelayCommand(ApplyWorldWindowLimitsForGeometryEditor1));
            }
        }

        public RelayCommand ZoomInForGeometryEditor1Command
        {
            get
            {
                return _zoomInForGeometryEditor1Command ?? (_zoomInForGeometryEditor1Command = new RelayCommand(ZoomInForGeometryEditor1));
            }
        }

        public RelayCommand ZoomOutForGeometryEditor1Command
        {
            get
            {
                return _zoomOutForGeometryEditor1Command ?? (_zoomOutForGeometryEditor1Command = new RelayCommand(ZoomOutForGeometryEditor1));
            }
        }

        public RelayCommand SetSelectedRegionForGeometryEditor1Command
        {
            get
            {
                return _setSelectedRegionForGeometryEditor1Command ?? (_setSelectedRegionForGeometryEditor1Command = new RelayCommand(SetSelectedRegionForGeometryEditor1));
            }
        }

        public RelayCommand SetWorldWindowForGeometryEditor1Command
        {
            get
            {
                return _setWorldWindowForGeometryEditor1Command ?? (_setWorldWindowForGeometryEditor1Command = new RelayCommand(SetWorldWindowForGeometryEditor1));
            }
        }

        public RelayCommand ZoomInForGeometryEditor2Command
        {
            get
            {
                return _zoomInForGeometryEditor2Command ?? (_zoomInForGeometryEditor2Command = new RelayCommand(ZoomInForGeometryEditor2));
            }
        }

        public RelayCommand ZoomOutForGeometryEditor2Command
        {
            get
            {
                return _zoomOutForGeometryEditor2Command ?? (_zoomOutForGeometryEditor2Command = new RelayCommand(ZoomOutForGeometryEditor2));
            }
        }

        public GeometryEditorViewModel GeometryEditorViewModel1 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel2 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel3 { get; private set; }
        public GeometryEditorViewModel GeometryEditorViewModel4 { get; private set; }
        public CoordinateSystemViewModel CoordinateSystemViewModel1 { get; private set; }
        public TimeSeriesViewModel TimeSeriesViewModel1 { get; private set; }
        public TimeSeriesViewModel TimeSeriesViewModel2 { get; private set; }
        public TimeSeriesViewModel TimeSeriesViewModel3 { get; private set; }
        public ImageEditorViewModel ImageEditorViewModel { get; private set; }

        public Tab3ViewModel()
        {
            WindMillInHouseDrawingsRotates = false;
            AllowROISelectionForGeometryEditorViewModel1 = true;

            var worldWindowFocus = new Point(
                (_x1 + _x0) / 2,
                (_y1 + _y0) / 2);

            var worldWindowSize = new Size(
                _x1 - _x0,
                _y1 - _y0);

            InitializeGeometryEditorViewModel1();
            InitializeGeometryEditorViewModel2();
            InitializeGeometryEditorViewModel3(worldWindowFocus, worldWindowSize);
            InitializeGeometryEditorViewModel4(worldWindowFocus, worldWindowSize);

            DrawAHouse(GeometryEditorViewModel1);
            DrawAHouse(GeometryEditorViewModel2);

            DrawACoordinateSystem(GeometryEditorViewModel3);

            var now = DateTime.UtcNow;
            _timeStampsOfInterest = new List<DateTime>
            {
                now,
                now - TimeSpan.FromMinutes(30),
                now - TimeSpan.FromMinutes(45),
                new (2014, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new (2015, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new (2016, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        private void DrawAHouse(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            var wwLimitBrush = new SolidColorBrush(Colors.BlanchedAlmond);

            // 5 tiles illustrating world window limits
            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(-100, -100),
                    new PointD(-100, 600),
                    new PointD(600, 600),
                    new PointD(600, -100)
                },
                3.0,
                wwLimitBrush);

            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(-100 - 700, -100),
                    new PointD(-100 - 700, 600),
                    new PointD(600 - 700, 600),
                    new PointD(600 - 700, -100)
                },
                3.0,
                wwLimitBrush);

            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(-100 + 700, -100),
                    new PointD(-100 + 700, 600),
                    new PointD(600 + 700, 600),
                    new PointD(600 + 700, -100)
                },
                3.0,
                wwLimitBrush);

            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(-100, -100 - 700),
                    new PointD(-100, 600 - 700),
                    new PointD(600, 600 - 700),
                    new PointD(600, -100 - 700)
                },
                3.0,
                wwLimitBrush);

            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(-100, -100 + 700),
                    new PointD(-100, 600 + 700),
                    new PointD(600, 600 + 700),
                    new PointD(600, -100 + 700)
                },
                3.0,
                wwLimitBrush);

            // Frame
            var frameBrush = new SolidColorBrush(Colors.DarkRed);
            geometryEditorViewModel.AddPolygon(new List<PointD>
                {
                    new PointD(0, 0),
                    new PointD(0, 200),
                    new PointD(200, 300),
                    new PointD(400, 200),
                    new PointD(400, 0)
                },
                3.0,
                frameBrush);

            // Door
            var doorAndWindowFrameBrush = new SolidColorBrush(Colors.GhostWhite);

            geometryEditorViewModel.AddPolyline(new List<PointD>
                {
                    new PointD(50, 0),
                    new PointD(50, 150),
                    new PointD(150, 150),
                    new PointD(150, 0)
                },
                2.0,
                doorAndWindowFrameBrush);

            geometryEditorViewModel.AddShape(1, new RectangleViewModel
            {
                Point = new PointD(100, 75),
                Width = 95,
                Height = 145,
                Text = "Door"
            });

            // Window
            geometryEditorViewModel.AddPolyline(new List<PointD>
            {
                new PointD(250, 75),
                new PointD(250, 150),
                new PointD(350, 150),
                new PointD(350, 75),
                new PointD(250, 75)
            }, 2.0, doorAndWindowFrameBrush);

            geometryEditorViewModel.AddShape(2, new RectangleViewModel
            {
                Point = new PointD(300, 112.5),
                Width = 95,
                Height = 70,
                Text = "Window"
            });

            // Sun
            var sunRayBrush = new SolidColorBrush(Colors.DarkOrange);

            geometryEditorViewModel.AddPoint(new PointD(385, 415), 10);
            geometryEditorViewModel.AddPoint(new PointD(415, 415), 10);
            geometryEditorViewModel.AddShape(3, new EllipseViewModel
            {
                Point = new PointD(400, 400),
                Width = 80,
                Height = 80,
                Text = "Sun"
            });

            geometryEditorViewModel.AddLine(new PointD(300, 400), new PointD(500, 400), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(400, 300), new PointD(400, 500), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(330, 330), new PointD(470, 470), 2, sunRayBrush);
            geometryEditorViewModel.AddLine(new PointD(330, 470), new PointD(470, 330), 2, sunRayBrush);

            // Label
            geometryEditorViewModel.AddLabel("Danshøjvej 33", new PointD(200, 300), 120, 40, new PointD(0, 20), 0.25);

            // Windmill
            geometryEditorViewModel.AddShape(4, new RectangleViewModel
            {
                Point = new PointD(500, 125),
                Width = 10.0,
                Height = 250.0
            });

            geometryEditorViewModel.AddShape(5, new RotatableEllipseViewModel
            {
                Point = new PointD(500, 250),
                Width = 100.0,
                Height = 10.0,
                Orientation = System.Math.PI / 4,
            });

            // Make the windmill rotate
            geometryEditorViewModel.UpdateModelCallBack = () =>
            {
                if (!WindMillInHouseDrawingsRotates)
                {
                    return;
                }

                var now = DateTime.Now;
                var fraction = now.Millisecond / 1000.0;
                var orientation = 2 * System.Math.PI * fraction;

                geometryEditorViewModel.ShapeViewModels
                    .Where(_ => _ is RotatableEllipseViewModel)
                    .Select(_ => _ as RotatableEllipseViewModel)
                    .ToList()
                    .ForEach(_ => _.Orientation = orientation);
            };
        }

        private void DrawACoordinateSystem(
            GeometryEditorViewModel geometryEditorViewModel)
        {
            // X Axis
            geometryEditorViewModel.AddLine(new PointD(-6, 0), new PointD(6, 0), 1.0, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(5.7, -0.2), new PointD(6, 0), 1.0, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(5.7, 0.2), new PointD(6, 0), 1.0, _coordinateSystemBrush);

            // Y Axis
            geometryEditorViewModel.AddLine(new PointD(0, -6), new PointD(0, 6), 1.0, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(-0.2, 5.7), new PointD(0, 6), 1.0, _coordinateSystemBrush);
            geometryEditorViewModel.AddLine(new PointD(0, 6), new PointD(0.2, 5.7), 1.0, _coordinateSystemBrush);

            // Axis ticks
            for (var n = 1; n <= 5; n++)
            {
                geometryEditorViewModel.AddLine(new PointD(n, -0.1), new PointD(n, 0.1), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-n, -0.1), new PointD(-n, 0.1), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLabel(n.ToString(), new PointD(n, -0.1), 40, 40, new PointD(0, 20), 0);
                geometryEditorViewModel.AddLabel((-n).ToString(), new PointD(-n, -0.1), 40, 40, new PointD(0, 20), 0);

                geometryEditorViewModel.AddLine(new PointD(-0.1, n), new PointD(0.1, n), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(-0.1, -n), new PointD(0.1, -n), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLabel(n.ToString(), new PointD(-0.1, n), 40, 40, new PointD(-20, 0), 0);
                geometryEditorViewModel.AddLabel((-n).ToString(), new PointD(-0.1, -n), 40, 40, new PointD(-20, 0), 0);
            }

            // Draw a window for diagnostics
            if (false)
            {
                geometryEditorViewModel.AddLine(new PointD(_x0, _y0), new PointD(_x1, _y0), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x1, _y0), new PointD(_x1, _y1), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x1, _y1), new PointD(_x0, _y1), 1.0, _coordinateSystemBrush);
                geometryEditorViewModel.AddLine(new PointD(_x0, _y1), new PointD(_x0, _y0), 1.0, _coordinateSystemBrush);
            }
        }

        private void ApplyWorldWindowLimitsForGeometryEditor1()
        {
            var left = WorldWindowLimitLeftForGeometryEditorViewModel1;
            var right = WorldWindowLimitRightForGeometryEditorViewModel1;
            var top = WorldWindowLimitTopForGeometryEditorViewModel1;
            var bottom = WorldWindowLimitBottomForGeometryEditorViewModel1;

            GeometryEditorViewModel1.WorldWindowUpperLeftLimit = new Point(left, top);
            GeometryEditorViewModel1.WorldWindowBottomRightLimit = new Point(right, bottom);
        }

        private void ZoomInForGeometryEditor1()
        {
            GeometryEditorViewModel1.ChangeScaling(1.2);
        }

        private void ZoomOutForGeometryEditor1()
        {
            GeometryEditorViewModel1.ChangeScaling(1 / 1.2);
        }

        private void SetSelectedRegionForGeometryEditor1()
        {
            var roiWidth = 95.0 * 1.5;
            var roiHeight = 70.0 * 1.5;

            GeometryEditorViewModel1.SelectedRegion.Object = new BoundingBox
            {
                Left = 300 - roiWidth / 2,
                Top = 112.5 - roiHeight / 2,
                Width = roiWidth,
                Height = roiHeight
            };
        }

        private void SetWorldWindowForGeometryEditor1()
        {
            var wwWidth = 95.0 * 1.5;
            var wwHeight = 70.0 * 1.5;

            var focus = new Point(300, 112.5);
            var size = new Size(wwWidth, wwHeight);

            GeometryEditorViewModel1.InitializeWorldWindow(focus, size, false);
        }

        private void ZoomInForGeometryEditor2()
        {
            GeometryEditorViewModel2.ChangeScaling(1.2);
        }

        private void ZoomOutForGeometryEditor2()
        {
            GeometryEditorViewModel2.ChangeScaling(1 / 1.2);
        }

        private void UpdateCoordinateSystemForGeometryEditorViewModel4(
            double x0,
            double x1,
            double y0,
            double y1)
        {
            var dx = 20 / GeometryEditorViewModel4.Scaling.Width;
            var dy = 20 / GeometryEditorViewModel4.Scaling.Height;

            GeometryEditorViewModel4.ClearLines();
            GeometryEditorViewModel4.ClearLabels();

            // 1: Find ud af spacing af ticks for x-aksen
            var spacingX = 1.0;
            var labelWidth = spacingX * GeometryEditorViewModel4.Scaling.Width;
            var labelHeight = 20.0;

            // Find ud af første x-værdi
            var x = System.Math.Floor(x0 / spacingX) * spacingX;

            while (x < x1)
            {
                if (x > x0 + dx)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x, y0 + dy),
                            new PointD(x, y1),
                            1.0,
                            _gridBrush);
                    }
                    
                    if(_includeTicks)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x, y0 + dy * 0.8),
                            new PointD(x, y0 + dy * 1.2),
                            1.0,
                            _coordinateSystemBrush);
                    }

                    var text = x.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel4.AddLabel(
                        text,
                        new PointD(x, y0 + dy),
                        labelWidth,
                        labelHeight,
                        new PointD(0, labelHeight / 2),
                        0.0);
                }

                x += spacingX;
            }

            // 1: Find ud af spacing af ticks for y-aksen
            var spacingY = 1.0;

            // Find ud af første y-værdi
            var y = System.Math.Floor(y0 / spacingY) * spacingY;

            while (y < y1)
            {
                if (y > y0 + dy)
                {
                    if (_includeGrid)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x0 + dx, y),
                            new PointD(x1, y),
                            1.0,
                            _gridBrush);
                    }

                    if (_includeTicks)
                    {
                        GeometryEditorViewModel4.AddLine(
                            new PointD(x0 + dx * 0.8, y),
                            new PointD(x0 + dx * 1.2, y),
                            1.0,
                            _coordinateSystemBrush);
                    }

                    var text = y.ToString(CultureInfo.InvariantCulture);

                    GeometryEditorViewModel4.AddLabel(
                        text,
                        new PointD(x0 + dx * 0.8, y),
                        20,
                        20,
                        new PointD(-10, 0),
                        0.0);
                }

                y += spacingY;
            }
        }

        private void InitializeGeometryEditorViewModel1()
        {
            GeometryEditorViewModel1 = new GeometryEditorViewModel
            {
                SelectRegionPossible = AllowROISelectionForGeometryEditorViewModel1,
                //SelectedRegionLimitedVertically = false
                SelectedRegionLimitedVertically = true
            };

            WorldWindowLimitLeftForGeometryEditorViewModel1 = -100.0;
            WorldWindowLimitRightForGeometryEditorViewModel1 = 600.0;
            WorldWindowLimitTopForGeometryEditorViewModel1 = -100.0;
            WorldWindowLimitBottomForGeometryEditorViewModel1 = 600.0;

            // Diagnostics
            GeometryEditorViewModel1.WorldWindowUpperLeftLimit = new Point(
                WorldWindowLimitLeftForGeometryEditorViewModel1,
                WorldWindowLimitTopForGeometryEditorViewModel1);

            GeometryEditorViewModel1.WorldWindowBottomRightLimit = new Point(
                WorldWindowLimitRightForGeometryEditorViewModel1, 
                WorldWindowLimitBottomForGeometryEditorViewModel1);

            GeometryEditorViewModel1.SelectedRegion.PropertyChanged += (s, e) =>
            {
                var left = GeometryEditorViewModel1.SelectedRegion.Object.Left;
                var right = GeometryEditorViewModel1.SelectedRegion.Object.Right;
                var top = GeometryEditorViewModel1.SelectedRegion.Object.Top;
                var bottom = GeometryEditorViewModel1.SelectedRegion.Object.Bottom;
                ROIXAsText = $"X: {left:F2} - {right:F2}";
                ROIYAsText = $"Y: {top:F2} - {bottom:F2}";
            };
        }

        private void InitializeGeometryEditorViewModel2()
        {
            GeometryEditorViewModel2 = new GeometryEditorViewModel(-1);
            GeometryEditorViewModel2.InitializeWorldWindow(new Point(300, 112.5));

            //GeometryEditorViewModel2.WorldWindowUpperLeftLimit = new Point(-100, -600);
            //GeometryEditorViewModel2.WorldWindowBottomRightLimit = new Point(600, 100);

            //GeometryEditorViewModel2.XScalingLocked = true;
            //GeometryEditorViewModel2.YScalingLocked = false;
        }

        private void InitializeGeometryEditorViewModel3(
            Point worldWindowFocus,
            Size worldWindowSize)
        {
            GeometryEditorViewModel3 = new GeometryEditorViewModel(-1);

            GeometryEditorViewModel3.InitializeWorldWindow(
                worldWindowFocus,
                worldWindowSize,
                false);

            GeometryEditorViewModel3.WorldWindowMajorUpdateOccured += (s, e) =>
            {
                var x0 = System.Math.Floor(e.WorldWindowUpperLeft.X);
                var x1 = System.Math.Ceiling(x0 + e.WorldWindowSize.Width);

                var points = new List<PointD>();
                for (var x = x0; x <= x1; x += 0.1)
                {
                    //points.Add(new PointD(x, x));                                                         // y = x
                    //points.Add(new PointD(x, 0.5 * x));                                                   // y = 0.5x
                    //points.Add(new PointD(x, 0.5 * x - 1));                                               // y = 0.5x - 1
                    //points.Add(new PointD(x, -x));                                                        // y = -x
                    //points.Add(new PointD(x, 0));                                                         // y = 0
                    //points.Add(new PointD(x, 2));                                                         // y = 2
                    //points.Add(new PointD(x, x * x));                                                     // y = x^2
                    //points.Add(new PointD(x, x * x * x));                                                 // y = x^3
                    //points.Add(new PointD(x, Math.Abs(x)));                                               // y = |x|
                    //points.Add(new PointD(x, -x * x));                                                    // y = -x^2
                    //points.Add(new PointD(x, Math.Pow(x - 2, 2) - 3));                                    // y = (x - 2)^2 - 3 = x^2 - 4x + 1
                    points.Add(new PointD(x, System.Math.Pow(x, 3) / 4 + 3 * System.Math.Pow(x, 2) / 4 - 3 * x / 2 - 2)); // y = 0.25x^3 + 0.75x^2 - 1.5x - 2
                                                                                                            //points.Add(new PointD(x, Math.Sin(x)));                                               // y = sin(x)
                }

                GeometryEditorViewModel3.ClearPolylines();

                var curveThickness = 0.03;

                GeometryEditorViewModel3.AddPolyline(points, curveThickness, _curveBrush);
            };

            GeometryEditorViewModel3.MousePositionWorld.PropertyChanged += (s, e) =>
            {
                CursorPositionForGeometryEditorViewModel3AsText = GeometryEditorViewModel3.MousePositionWorld.Object.HasValue
                    ? $"({GeometryEditorViewModel3.MousePositionWorld.Object.Value.X:N2}, {-GeometryEditorViewModel3.MousePositionWorld.Object.Value.Y:N2})"
                    : "";
            };
        }

        private void InitializeGeometryEditorViewModel4(
            Point worldWindowFocus,
            Size worldWindowSize)
        {
            GeometryEditorViewModel4 = new GeometryEditorViewModel(-1);

            GeometryEditorViewModel4.InitializeWorldWindow(
                worldWindowFocus,
                worldWindowSize,
                false);

            GeometryEditorViewModel4.WorldWindowUpdateOccured += (s, e) => 
            {
                WorldWindowUpdateCountForGeometryEditorViewModel4++;
                GeometryEditorViewModel4.ClearLabels();
            };

            GeometryEditorViewModel4.WorldWindowMajorUpdateOccured += (s, e) => 
            {
                WorldWindowMajorUpdateCountForGeometryEditorViewModel4++;

                // Notice that world window coordinates are always given in "non-inverted" coordinates,
                // so we need to invert the y coordinate
                UpdateCoordinateSystemForGeometryEditorViewModel4(
                    e.WorldWindowUpperLeft.X,
                    e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width,
                    -e.WorldWindowUpperLeft.Y - e.WorldWindowSize.Height,
                    -e.WorldWindowUpperLeft.Y);

                var x0 = System.Math.Floor(e.WorldWindowUpperLeft.X);
                var x1 = System.Math.Ceiling(e.WorldWindowUpperLeft.X + e.WorldWindowSize.Width);

                var points = new List<PointD>();
                for (var x = x0; x <= x1; x += 0.1)
                {
                    //points.Add(new PointD(x, Math.Pow(x, 3) / 4 + 3 * Math.Pow(x, 2) / 4 - 3 * x / 2 - 2)); // y = 0.25x^3 + 0.75x^2 - 1.5x - 2
                    points.Add(new PointD(x, System.Math.Exp(-0.01 * x * x) * System.Math.Sin(3 * x))); // (gaussian and sinus)
                }

                GeometryEditorViewModel4.ClearPolylines();

                var curveThickness = 0.03;
                GeometryEditorViewModel4.AddPolyline(points, curveThickness, _curveBrush);
            };
        }
    }
}
