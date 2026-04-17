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
        private BoundingBox _expandedWorldWindow;
        private System.Windows.Point? _cursorWorldPosition;
        private bool _lockAspectRatio;
        private bool _lockXAxis;
        private bool _lockYAxis;

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
                UpdateExpandedWorldWindowIfNeeded();
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

        public ObservableCollection<LineModel> Lines { get; }
            = new ObservableCollection<LineModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public GeometryViewModel()
        {
            LockAspectRatio = true;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void UpdateExpandedWorldWindowIfNeeded()
        {
            if (_expandedWorldWindow == null ||
                !Contains(ExpandedWorldWindow, WorldWindow) ||
                ExpandedWorldWindow.Width / WorldWindow.Width > 2.0)
            {
                ExpandedWorldWindow = Expand(WorldWindow, 1.2);
            }
        }

        private BoundingBox Expand(
            BoundingBox box,
            double factor)
        {
            var width = box.Width;
            var height = box.Height;

            var expandX = width * (factor - 1) / 1.2;
            var expandY = height * (factor - 1) / 1.2;

            return new BoundingBox(
                box.MinX - expandX,
                box.MaxX + expandX,
                box.MinY - expandY,
                box.MaxY + expandY);
        }

        private bool Contains(
            BoundingBox bb1,
            BoundingBox bb2)
        {
            if (bb1.MinX <= bb2.MinX &&
                bb2.MaxX <= bb1.MaxX &&
                bb1.MinY <= bb2.MinY &&
                bb2.MaxY <= bb1.MaxY)
            {
                return true;
            }

            return false;
        }
    }
}
