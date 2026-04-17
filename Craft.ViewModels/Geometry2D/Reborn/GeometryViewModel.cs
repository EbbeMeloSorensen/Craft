using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Craft.DataStructures.Geometry;

namespace Craft.ViewModels.Geometry2D.Reborn
{
    public class GeometryViewModel : INotifyPropertyChanged
    {
        private ViewState _viewState;
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
    }
}
