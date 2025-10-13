using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Craft.Math;

namespace Craft.GeometryEditor3D.ViewModel
{
    public class GeometryEditorViewModel : ViewModelBase
    {
        private enum ViewingMode
        {
            StationaryCamera,
            Focusing
        }

        private enum Projection
        {
            Parallel,
            Perspective
        }

        private enum OutCode
        {
            Left = 1,
            Right = 2,
            Bottom = 4,
            Top = 8,
            Back = 16,
            Front = 32
        }

        private ViewingMode _viewingMode;
        private Projection _projection;

        private Vector3D _focus;

        // Practical view volume definition
        private Size _viewPortSize;
        private double _horizontalViewAngle;
        private double _magnification;
        private Vector3D _cameraPosition;
        private double _distanceFromCameraToFocus;

        // View volume definition from "Computer Graphics, Principles and Practice" Book
        private Vector3D _VRP;
        private Vector3D _VPN;
        private Vector3D _VUP;
        private Vector3D _PRP;
        private Vector2D _CW;
        private double _umin;
        private double _umax;
        private double _vmin;
        private double _vmax;
        private double _B;
        private double _F;

        // Derived by view volume definition
        private double _zproj;
        private double _zmin;

        private List<Point3D> _points3D;
        private List<Point2D> _points2D;

        private List<LineSegment3D> _lines3D;
        private List<LineSegment2D> _lines2D;

        private Vector3D _vpnDragStart;
        private Vector3D _vupDragStart;
        private Vector3D _horizontalAxisDragStart;
        
        private Matrix _transformationMatrix3DToWorldWindow;

        public Size ViewPortSize
        {
            get { return _viewPortSize; }
            set
            {
                _viewPortSize = value;
                UpdateTransformationMatrix3DToWorldWindow();
                RaisePropertyChanged();
            }
        }

        public double HorizontalViewAngle
        {
            get { return _horizontalViewAngle; }
            set
            {
                _horizontalViewAngle = value;
                UpdateTransformationMatrix3DToWorldWindow();
                RaisePropertyChanged();
            }
        }

        public double Magnification
        {
            get { return _magnification; }
            set
            {
                _magnification = value;
                UpdateTransformationMatrix3DToWorldWindow();
                RaisePropertyChanged();
            }
        }

        public Vector3D CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                _cameraPosition = value;
                UpdateTransformationMatrix3DToWorldWindow();
                RaisePropertyChanged();
            }
        }

        public Vector3D Focus
        {
            get { return _focus; }
            set
            {
                _focus = value;
                UpdateTransformationMatrix3DToWorldWindow();
                RaisePropertyChanged();
            }
        }

        public Vector3D VRP
        {
            get { return _VRP; }
            set
            {
                _VRP = value;
                RaisePropertyChanged();
            }
        }

        public Vector3D VPN
        {
            get { return _VPN; }
            set
            {
                _VPN = value;
                RaisePropertyChanged();
            }
        }

        public Vector3D VUP
        {
            get { return _VUP; }
            set
            {
                _VUP = value;
                RaisePropertyChanged();
            }
        }

        public Vector3D PRP
        {
            get { return _PRP; }
            set
            {
                _PRP = value;
                RaisePropertyChanged();
            }
        }

        public Vector2D CW
        {
            get { return _CW; }
            set
            {
                _CW = value;
                RaisePropertyChanged();
            }
        }

        public double Umin
        {
            get { return _umin; }
            set
            {
                _umin = value;
                RaisePropertyChanged();
            }
        }

        public double Umax
        {
            get { return _umax; }
            set
            {
                _umax = value;
                RaisePropertyChanged();
            }
        }

        public double Vmin
        {
            get { return _vmin; }
            set
            {
                _vmin = value;
                RaisePropertyChanged();
            }
        }

        public double Vmax
        {
            get { return _vmax; }
            set
            {
                _vmax = value;
                RaisePropertyChanged();
            }
        }

        public double B
        {
            get { return _B; }
            set
            {
                _B = value;
                RaisePropertyChanged();
            }
        }

        public double F
        {
            get { return _F; }
            set
            {
                _F = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PointViewModel> PointViewModels { get; private set; }

        public ObservableCollection<LineViewModel> LineViewModels { get; private set; }

        public GeometryEditorViewModel()
        {
            _points3D = new List<Point3D>();
            _lines3D = new List<LineSegment3D>();
            _points2D = new List<Point2D>();
            _lines2D = new List<LineSegment2D>();

            PointViewModels = new ObservableCollection<PointViewModel>();
            LineViewModels = new ObservableCollection<LineViewModel>();

            // Initialize the view volume
            _horizontalViewAngle = 0.3;
            _magnification = 20.0;
            _cameraPosition = new Vector3D(8, 8, 100);
            _focus = new Vector3D(8, 8, 42);
            _distanceFromCameraToFocus = (_focus - _cameraPosition).Length;
            _viewingMode = ViewingMode.Focusing;

            _VPN = new Vector3D(0, 0, 1);
            _VUP = new Vector3D(0, 1, 0);
            _CW = new Vector2D(0, 0);
            _B = -100000;
            _F = 0;

            // Add stuff to the scene
            GenerateCoordinateSystem(20);
            GenerateHouseFromGraphicsBook(1);
            GenerateCubeCenteredAtOrigo(100);
            GenerateHouseFromGraphicsBook(1);
            //GenerateCircle(new Vector3D(0, 0, 0), 50, 30);
        }

        public void InitiateCameraRotationModification()
        {
            _vpnDragStart = VPN;
            _vupDragStart = VUP;
            _horizontalAxisDragStart = Vector3D.CrossProduct(VUP, VPN);
        }

        public void UpdateCameraOrientation(
            double rotationLeftRight, 
            double rotationUpDown)
        {
            var divisor = 200;

            var m1 = Matrix.GenerateRotationMatrix(_vupDragStart, rotationLeftRight / divisor);
            var m2 = Matrix.GenerateRotationMatrix(_horizontalAxisDragStart, rotationUpDown / divisor);
            var rotationMatrix = m1 * m2;

            VPN = (rotationMatrix * _vpnDragStart.AsVector()).AsVector3D();
            VUP = (rotationMatrix * _vupDragStart.AsVector()).AsVector3D();
            UpdateTransformationMatrix3DToWorldWindow();
        }

        private void UpdateTransformationMatrix3DToWorldWindow()
        {
            Umax = ViewPortSize.Width / 2 / Magnification;
            Umin = -Umax;
            Vmax = ViewPortSize.Height / 2 / Magnification;
            Vmin = -Vmax;

            if (_viewingMode == ViewingMode.Focusing)
            {
                _cameraPosition = Focus + VPN * _distanceFromCameraToFocus;
            }

            VRP = CameraPosition;

            var prpz = 1.0;

            if (System.Math.Abs(_horizontalViewAngle - 0.0) < 0.000001)
            {
                _projection = Projection.Parallel;
            }
            else
            {
                _projection = Projection.Perspective;
                prpz = _umax / System.Math.Tan(_horizontalViewAngle / 2);
            }

            PRP = new Vector3D(0, 0, prpz);
            var DOP = CW.AsVector3D() - PRP;
            var shxpar = -DOP.X / DOP.Z;
            var shypar = -DOP.Y / DOP.Z;

            // Step 1: Tranlation matrix
            var T = Matrix.GenerateTranslationMatrix(-VRP);

            // Step 2: Rotation matrix
            var R = Matrix.GenerateRotationMatrix(VPN, VUP);

            // Step 3: Shear matrix
            var SHpar = Matrix.GenerateXYShearMatrix(shxpar, shypar);

            if (_projection == Projection.Parallel)
            {
                // Step 4: Translation matrix
                var Tpar = Matrix.GenerateTranslationMatrix(new Vector3D(-(Umax + Umin) / 2, -(Vmax + Vmin) / 2, -F));

                // Step 5: Scaling matrix
                var Spar = Matrix.GenerateScalingMatrix(2 / (Umax - Umin), 2 / (Vmax - Vmin), 1 / (F - B));

                _transformationMatrix3DToWorldWindow = Spar * Tpar * SHpar * R * T;
            }
            else
            {
                var T2 = Matrix.GenerateTranslationMatrix(-PRP);

                var vrpz = -PRP.Z;
                var sx = 2 * vrpz / (Umax - Umin) / (vrpz + B);
                var sy = 2 * vrpz / (Vmax - Vmin) / (vrpz + B);
                var sz = -1 / (vrpz + B);

                var Sper = Matrix.GenerateScalingMatrix(sx, sy, sz);

                _transformationMatrix3DToWorldWindow = Sper * SHpar * T2 * R * T;

                _zproj = -vrpz / (vrpz + B);
                _zmin = -(vrpz + F) / (vrpz + B);
            }

            UpdatePointViewModelCollection();
            UpdateLineViewModelCollection();
        }

        private void UpdatePointViewModelCollection()
        {
            _points2D.Clear();

            switch (_projection)
            {
                case Projection.Parallel:
                    _points3D
                        .Select(p => TransformFromWorldSpaceIntoCanonicalSpace(p))
                        .ToList()
                        .ForEach(v =>
                        {
                            var x = v[0];
                            var y = v[1];
                            var z = v[2];
                            var outCode = ComputeOutCodeParallelProjectionCanonicalViewVolume(x, y, z);

                            if (outCode == 0)
                            {
                                _points2D.Add(TransformFromCanonicalParallelProjectionSpaceIntoViewport(x, y));
                            }
                        });
                    break;
                case Projection.Perspective:
                    _points3D
                        .Select(p => TransformFromWorldSpaceIntoCanonicalSpace(p))
                        .ToList()
                        .ForEach(v =>
                        {
                            var x = v[0];
                            var y = v[1];
                            var z = v[2];
                            var outCode = ComputeOutCodePerspectiveProjectionCanonicalViewVolume(x, y, z);

                            if (outCode == 0)
                            {
                                _points2D.Add(TransformFromCanonicalPerspectiveProjectionSpaceIntoViewport(x, y, z));
                            }
                        });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Det er sgu en lille smule mærkeligt, at den ikke samler dette op,
            // og at jeg derfor må lave flik-flakket nedenfor
            // Hvordan er det lige du gør det samme i ContactManageren?

            //PointViewModels = new ObservableCollection<PointViewModel>(_points2D.Select(p => new PointViewModel(p)));
            PointViewModels.Clear();
            _points2D.ForEach(p => PointViewModels.Add(new PointViewModel(p)));
        }

        private void UpdateLineViewModelCollection()
        {
            _lines2D.Clear();

            switch (_projection)
            {
                case Projection.Parallel:
                    _lines3D
                        .Select(l => TransformFromWorldSpaceIntoCanonicalSpace(l))
                        .ToList()
                        .ForEach(vp =>
                        {
                            var v1 = vp.Item1;
                            var v2 = vp.Item2;
                            var x0 = v1[0];
                            var y0 = v1[1];
                            var z0 = v1[2];
                            var x1 = v2[0];
                            var y1 = v2[1];
                            var z1 = v2[2];
                            var accept = false;
                            var done = false;
                            var outCode1 = ComputeOutCodeParallelProjectionCanonicalViewVolume(x0, y0, z0);
                            var outCode2 = ComputeOutCodeParallelProjectionCanonicalViewVolume(x1, y1, z1);

                            do
                            {
                                if ((outCode1 | outCode2) == 0)
                                {
                                    // Trivial accept (both points are within the view volume)
                                    accept = true;
                                    done = true;
                                }
                                else if ((outCode1 & outCode2) > 0)
                                {
                                    // Trivial reject (both points are behind one of the planes defining the view volume)
                                    done = true;
                                }
                                else
                                {
                                    var x = 0.0;
                                    var y = 0.0;
                                    var z = 0.0;
                                    var outcodeOut = outCode1 > 0 ? outCode1 : outCode2;

                                    if ((outcodeOut & (uint)OutCode.Top) > 0)
                                    {
                                        var t = (1 - y0) / (y1 - y0);
                                        x = x0 + (x1 - x0) * t;
                                        y = 1;
                                        z = z0 + (z1 - z0) * t;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Bottom) > 0)
                                    {
                                        var t = (-1 - y0) / (y1 - y0);
                                        x = x0 + (x1 - x0) * t;
                                        y = -1;
                                        z = z0 + (z1 - z0) * t;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Left) > 0)
                                    {
                                        var t = (-1 - x0) / (x1 - x0);
                                        x = -1;
                                        y = y0 + (y1 - y0) * t;
                                        z = z0 + (z1 - z0) * t;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Right) > 0)
                                    {
                                        var t = (1 - x0) / (x1 - x0);
                                        x = 1;
                                        y = y0 + (y1 - y0) * t;
                                        z = z0 + (z1 - z0) * t;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Back) > 0)
                                    {
                                        var t = (-1 - z0) / (z1 - z0);
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = -1;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Front) > 0)
                                    {
                                        var t = (0 - z0) / (z1 - z0);
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = 0;
                                    }

                                    if (outcodeOut == outCode1)
                                    {
                                        x0 = x;
                                        y0 = y;
                                        z0 = z;
                                        outCode1 = ComputeOutCodeParallelProjectionCanonicalViewVolume(x0, y0, z0);
                                    }
                                    else
                                    {
                                        x1 = x;
                                        y1 = y;
                                        z1 = z;
                                        outCode2 = ComputeOutCodeParallelProjectionCanonicalViewVolume(x1, y1, z1);
                                    }
                                }
                            } while (!done);

                            if (accept)
                            {
                                var p1 = TransformFromCanonicalParallelProjectionSpaceIntoViewport(x0, y0);
                                var p2 = TransformFromCanonicalParallelProjectionSpaceIntoViewport(x1, y1);

                                _lines2D.Add(new LineSegment2D(p1, p2));
                            }
                        });
                    break;
                case Projection.Perspective:
                    _lines3D
                        .Select(l => TransformFromWorldSpaceIntoCanonicalSpace(l))
                        .ToList()
                        .ForEach(vp =>
                        {
                            var v1 = vp.Item1;
                            var v2 = vp.Item2;
                            var x0 = v1[0];
                            var y0 = v1[1];
                            var z0 = v1[2];
                            var x1 = v2[0];
                            var y1 = v2[1];
                            var z1 = v2[2];
                            var accept = false;
                            var done = false;
                            var outCode1 = ComputeOutCodePerspectiveProjectionCanonicalViewVolume(x0, y0, z0);
                            var outCode2 = ComputeOutCodePerspectiveProjectionCanonicalViewVolume(x1, y1, z1);

                            do
                            {
                                if ((outCode1 | outCode2) == 0)
                                {
                                    // Trivial accept (both points are within the view volume)
                                    accept = true;
                                    done = true;
                                }
                                else if ((outCode1 & outCode2) > 0)
                                {
                                    // Trivial reject (both points are behind one of the planes defining the view volume)
                                    done = true;
                                }
                                else
                                {
                                    var x = 0.0;
                                    var y = 0.0;
                                    var z = 0.0;
                                    var outcodeOut = outCode1 > 0 ? outCode1 : outCode2;

                                    if ((outcodeOut & (uint)OutCode.Top) > 0)
                                    {
                                        var t = (z0 + y0) / (y0 - y1 - (z1 - z0));
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = -y;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Bottom) > 0)
                                    {
                                        var t = (z0 - y0) / (y1 - y0 - (z1 - z0));
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = y;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Left) > 0)
                                    {
                                        var t = (z0 - x0) / (x1 - x0 - (z1 - z0));
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = x;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Right) > 0)
                                    {
                                        var t = (z0 + x0) / (x0 - x1 - (z1 - z0));
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = -x;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Back) > 0)
                                    {
                                        var t = (-z0 - 1) / (z1 - z0);
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = -1;
                                    }
                                    else if ((outcodeOut & (uint)OutCode.Front) > 0)
                                    {
                                        var t = (z0 - _zmin) / (z0 - z1);
                                        x = x0 + (x1 - x0) * t;
                                        y = y0 + (y1 - y0) * t;
                                        z = _zmin;
                                    }

                                    if (outcodeOut == outCode1)
                                    {
                                        x0 = x;
                                        y0 = y;
                                        z0 = z;
                                        outCode1 = ComputeOutCodePerspectiveProjectionCanonicalViewVolume(x0, y0, z0);
                                    }
                                    else
                                    {
                                        x1 = x;
                                        y1 = y;
                                        z1 = z;
                                        outCode2 = ComputeOutCodePerspectiveProjectionCanonicalViewVolume(x1, y1, z1);
                                    }
                                }
                            } while (!done);

                            if (accept)
                            {
                                var p1 = TransformFromCanonicalPerspectiveProjectionSpaceIntoViewport(x0, y0, z0);
                                var p2 = TransformFromCanonicalPerspectiveProjectionSpaceIntoViewport(x1, y1, z1);

                                _lines2D.Add(new LineSegment2D(p1, p2));
                            }
                        });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Det er sgu en lille smule mærkeligt, at den ikke samler dette op,
            // og at jeg derfor må lave flik-flakket nedenfor
            // Hvordan er det lige du gør det samme i ContactManageren?

            //LineViewModels = new ObservableCollection<LineViewModel>(_lines2D.Select(l => new LineViewModel(l)));
            LineViewModels.Clear();
            _lines2D.ForEach(l => LineViewModels.Add(new LineViewModel(l)));
        }

        private Math.Vector TransformFromWorldSpaceIntoCanonicalSpace(Point3D point3D)
        {
            return _transformationMatrix3DToWorldWindow * point3D.AsVectorInHomogeneousCoordinates();
        }

        private Tuple<Math.Vector, Math.Vector> TransformFromWorldSpaceIntoCanonicalSpace(LineSegment3D lineSegment3D)
        {
            return new Tuple<Math.Vector, Math.Vector>(
                _transformationMatrix3DToWorldWindow * lineSegment3D.Point1.AsVectorInHomogeneousCoordinates(),
                _transformationMatrix3DToWorldWindow * lineSegment3D.Point2.AsVectorInHomogeneousCoordinates());
        }

        private Point2D TransformFromCanonicalParallelProjectionSpaceIntoViewport(double x, double y)
        {
            return new Point2D(
                ViewPortSize.Width * (x + 1) / 2,
                ViewPortSize.Height * (1 - (y + 1) / 2));
        }

        private Point2D TransformFromCanonicalPerspectiveProjectionSpaceIntoViewport(double x, double y, double z)
        {
            return new Point2D(
                ViewPortSize.Width * (-x / z + 1) / 2,
                ViewPortSize.Height * (1 - (-y / z + 1) / 2));
        }

        private uint ComputeOutCodeParallelProjectionCanonicalViewVolume(
            double x, 
            double y,
            double z)
        {
            uint outcode = 0;

            if (x < -1)
            {
                outcode |= (uint)OutCode.Left;
            }
            else if (x > 1)
            {
                outcode |= (uint)OutCode.Right;
            }

            if (y < -1)
            {
                outcode |= (uint)OutCode.Bottom;
            }
            else if (y > 1)
            {
                outcode |= (uint)OutCode.Top;
            }

            if (z < -1)
            {
                outcode |= (uint)OutCode.Back;
            }
            else if (z > 0)
            {
                outcode |= (uint)OutCode.Front;
            }

            return outcode;
        }

        private uint ComputeOutCodePerspectiveProjectionCanonicalViewVolume(
            double x,
            double y,
            double z)
        {
            uint outcode = 0;

            if (x < z)
            {
                outcode |= (uint)OutCode.Left;
            }
            else if (x > -z)
            {
                outcode |= (uint)OutCode.Right;
            }

            if (y < z)
            {
                outcode |= (uint)OutCode.Bottom;
            }
            else if (y > -z)
            {
                outcode |= (uint)OutCode.Top;
            }

            if (z < -1)
            {
                outcode |= (uint)OutCode.Back;
            }
            else if (z > _zmin)
            {
                outcode |= (uint)OutCode.Front;
            }

            return outcode;
        }

        private void GenerateCubeCenteredAtOrigo(double size)
        {
            var halfSize = size / 2;

            var point1 = new Point3D(-halfSize, -halfSize, -halfSize);
            var point2 = new Point3D(-halfSize, halfSize, -halfSize);
            var point3 = new Point3D(halfSize, halfSize, -halfSize);
            var point4 = new Point3D(halfSize, -halfSize, -halfSize);
            var point5 = new Point3D(-halfSize, -halfSize, halfSize);
            var point6 = new Point3D(-halfSize, halfSize, halfSize);
            var point7 = new Point3D(halfSize, halfSize, halfSize);
            var point8 = new Point3D(halfSize, -halfSize, halfSize);

            _points3D.Add(point1);
            _points3D.Add(point2);
            _points3D.Add(point3);
            _points3D.Add(point4);
            _points3D.Add(point5);
            _points3D.Add(point6);
            _points3D.Add(point7);
            _points3D.Add(point8);

            _lines3D.Add(new LineSegment3D(point1, point2));
            _lines3D.Add(new LineSegment3D(point2, point3));
            _lines3D.Add(new LineSegment3D(point3, point4));
            _lines3D.Add(new LineSegment3D(point4, point1));
            _lines3D.Add(new LineSegment3D(point5, point6));
            _lines3D.Add(new LineSegment3D(point6, point7));
            _lines3D.Add(new LineSegment3D(point7, point8));
            _lines3D.Add(new LineSegment3D(point8, point5));
            _lines3D.Add(new LineSegment3D(point1, point5));
            _lines3D.Add(new LineSegment3D(point2, point6));
            _lines3D.Add(new LineSegment3D(point3, point7));
            _lines3D.Add(new LineSegment3D(point4, point8));
        }

        private void GenerateHouseFromGraphicsBook(double scale)
        {
            var center = new Point3D(8, 0, 42);
            var point1 = center + scale * new Point3D(-8, 0, -12);
            var point2 = center + scale * new Point3D(8, 0, -12);
            var point3 = center + scale * new Point3D(8, 0, 12);
            var point4 = center + scale * new Point3D(-8, 0, 12);
            var point5 = center + scale * new Point3D(-8, 10, -12);
            var point6 = center + scale * new Point3D(8, 10, -12);
            var point7 = center + scale * new Point3D(8, 10, 12);
            var point8 = center + scale * new Point3D(-8, 10, 12);
            var point9 = center + scale * new Point3D(0, 16, -12);
            var point10 = center + scale * new Point3D(0, 16, 12);

            _lines3D.Add(new LineSegment3D(point1, point2));
            _lines3D.Add(new LineSegment3D(point2, point3));
            _lines3D.Add(new LineSegment3D(point3, point4));
            _lines3D.Add(new LineSegment3D(point4, point1));
            _lines3D.Add(new LineSegment3D(point1, point5));
            _lines3D.Add(new LineSegment3D(point2, point6));
            _lines3D.Add(new LineSegment3D(point3, point7));
            _lines3D.Add(new LineSegment3D(point4, point8));
            _lines3D.Add(new LineSegment3D(point5, point8));
            _lines3D.Add(new LineSegment3D(point6, point7));
            _lines3D.Add(new LineSegment3D(point5, point9));
            _lines3D.Add(new LineSegment3D(point9, point6));
            _lines3D.Add(new LineSegment3D(point8, point10));
            _lines3D.Add(new LineSegment3D(point10, point7));
            _lines3D.Add(new LineSegment3D(point9, point10));
        }

        private void GenerateCircle(Vector3D center, double radius, int nPoints)
        {
            var x0 = radius;
            var y0 = 0.0;
            var z0 = 0.0;
            var pt1 = new Point3D(x0, y0, z0);

            for (var i = 1; i <= nPoints; i++)
            {
                var angle = 2.0 * i * System.Math.PI / nPoints;
                var x1 = radius * System.Math.Cos(angle);
                var y1 = 0;
                var z1 = radius * System.Math.Sin(angle);
                var pt2 = new Point3D(x1, y1, z1);

                _lines3D.Add(new LineSegment3D(pt1, pt2));
                pt1 = pt2;
            }
        }

        private void GenerateCoordinateSystem(double axisLength)
        {
            var point1 = new Point3D(0, 0, 0);
            var point2 = new Point3D(axisLength, 0, 0);
            var point3 = new Point3D(0, axisLength, 0);
            var point4 = new Point3D(0, 0, axisLength);

            _lines3D.Add(new LineSegment3D(point1, point2));
            _lines3D.Add(new LineSegment3D(point1, point3));
            _lines3D.Add(new LineSegment3D(point1, point4));
        }
    }
}
