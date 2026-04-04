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
        private Stopwatch _stopwatch;
        private string _roiXAsText;
        private string _roiYAsText;

        private RelayCommand _applyWorldWindowLimitsForGeometryEditor1Command;
        private RelayCommand _zoomInForGeometryEditor1Command;
        private RelayCommand _zoomOutForGeometryEditor1Command;
        private RelayCommand _setSelectedRegionForGeometryEditor1Command;
        private RelayCommand _setWorldWindowForGeometryEditor1Command;

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

        public GeometryEditorViewModel GeometryEditorViewModel1 { get; private set; }

        public Tab3ViewModel()
        {
            WindMillInHouseDrawingsRotates = false;
            AllowROISelectionForGeometryEditorViewModel1 = true;

            InitializeGeometryEditorViewModel1();

            DrawAHouse(GeometryEditorViewModel1);
            
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
    }
}
