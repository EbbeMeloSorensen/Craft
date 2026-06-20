using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn
{
    public class GeometryViewModel : INotifyPropertyChanged
    {
        private ViewState _viewState;
        private BoundingBox _worldWindow;
        private BoundingBox _worldWindowExpanded;
        private BoundingBox _requestedWorldWindow;
        private WorldFocusRequest _requestedWorldFocus;
        private BoundingBox _worldWindowBounds;
        private BoundingBox _expandedWorldWindow;
        private System.Windows.Point? _cursorWorldPosition;
        private bool _lockAspectRatio;
        private bool _lockXAxis;
        private bool _lockYAxis;
        private bool _dampFocusShifts;
        private double _focusShiftDamping;
        private bool _debugMode;
        private bool _showGrid;
        private bool _showCoordinateSystem;
        private bool _timeAxisMode;

        public ViewState ViewState
        {
            get => _viewState;
            set
            {
                _viewState = value;
                OnPropertyChanged();
            }
        }

        public BoundingBox WorldWindow
        {
            get => _worldWindow;
            set
            {
                _worldWindow = value;
                OnPropertyChanged();
            }
        }

        public BoundingBox WorldWindowExpanded
        {
            get => _worldWindowExpanded;
            set
            {
                _worldWindowExpanded = value;
                OnPropertyChanged();
            }
        }

        public BoundingBox ExpandedWorldWindow
        {
            get => _expandedWorldWindow;
            set
            {
                _expandedWorldWindow = value;
                OnPropertyChanged();
            }
        }

        public BoundingBox RequestedWorldWindow
        {
            get => _requestedWorldWindow;
            set
            {
                _requestedWorldWindow = value;
                OnPropertyChanged();
            }
        }

        public WorldFocusRequest RequestedWorldFocus
        {
            get => _requestedWorldFocus;
            set
            {
                _requestedWorldFocus = value;
                OnPropertyChanged();
            }
        }

        public BoundingBox WorldWindowBounds
        {
            get => _worldWindowBounds;
            set
            {
                _worldWindowBounds = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Point? CursorWorldPosition
        {
            get => _cursorWorldPosition;
            set
            {
                _cursorWorldPosition = value;
                OnPropertyChanged();
            }
        }

        public bool LockAspectRatio
        {
            get => _lockAspectRatio;
            set
            {
                _lockAspectRatio = value;
                OnPropertyChanged();

                if (_lockAspectRatio && TimeAxisMode)
                {
                    // Lock aspect ratio and time axis mode are incompatible, so disable
                    TimeAxisMode = false;
                }
            }
        }

        public bool LockXAxis
        {
            get => _lockXAxis;
            set
            {
                _lockXAxis = value;
                OnPropertyChanged();
            }
        }

        public bool LockYAxis
        {
            get => _lockYAxis;
            set
            {
                _lockYAxis = value;
                OnPropertyChanged();
            }
        }

        public bool DampFocusShifts
        {
            get => _dampFocusShifts;
            set
            {
                _dampFocusShifts = value;
                OnPropertyChanged();
            }
        }

        public double FocusShiftDamping
        {
            get => _focusShiftDamping;
            set
            {
                _focusShiftDamping = value;
                OnPropertyChanged();
            }
        }

        public bool DebugMode
        {
            get => _debugMode;
            set
            {
                _debugMode = value;
                OnPropertyChanged();
            }
        }

        public bool ShowGrid
        {
            get => _showGrid;
            set
            {
                _showGrid = value;
                OnPropertyChanged();
            }
        }

        public bool ShowCoordinateSystem
        {
            get => _showCoordinateSystem;
            set
            {
                _showCoordinateSystem = value;
                OnPropertyChanged();
            }
        }

        public bool TimeAxisMode
        {
            get => _timeAxisMode;
            set
            {
                _timeAxisMode = value;
                OnPropertyChanged();

                if (_timeAxisMode)
                {
                    // Lock aspect ratio and time axis mode are incompatible, so disable
                    LockAspectRatio = false;
                }
            }
        }

        public ObservableCollection<GeometryLayer> GeometryLayers { get; }
            = new ObservableCollection<GeometryLayer>();

        public event PropertyChangedEventHandler PropertyChanged;

        public GeometryViewModel()
        {
            WorldWindowBounds = new BoundingBox(
                double.MinValue,
                double.MaxValue,
                double.MinValue,
                double.MaxValue);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void ReplaceDynamicGeometryLayer(
            IEnumerable geometricObjects)
        {
            ClearLayer(true);

            GeometryLayers.Add(new GeometryLayer(geometricObjects, true));
        }

        public void AddStaticGeometryLayer(
            IEnumerable geometricObjects)
        {
            //ClearLayer(false);

            GeometryLayers.Add(new GeometryLayer(geometricObjects, false));
        }

        public void ClearLayer(
            bool frameDependent)
        {
            var remainingLayers = GeometryLayers
                .Where(gl => gl.IsFrameDependent != frameDependent)
                .ToList();

            GeometryLayers.Clear();

            remainingLayers.ForEach(gl => GeometryLayers.Add(gl));
        }
    }
}
