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
        private IGeometryDataSource _geometryDataSource;

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
                UpdateLineCollection();
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
                //UpdateLineCollection();
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

        public ObservableCollection<LineModel> Lines { get; }
            = new ObservableCollection<LineModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public GeometryViewModel()
        {
            //_geometryDataSource = new SimpleGeometryDataSource();
            //_geometryDataSource = new FunctionCurveDataSource();
            _geometryDataSource = new MxCifQuadTreeGeometryDataSource();

            LockAspectRatio = true;
            DampFocusShifts = true;
            FocusShiftDamping = 5.0;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void UpdateLineCollection()
        {
            Lines.Clear();

            _geometryDataSource
                .Query(WorldWindowExpanded)
                .ToList()
                .ForEach(line => Lines.Add(line));
        }
    }
}
