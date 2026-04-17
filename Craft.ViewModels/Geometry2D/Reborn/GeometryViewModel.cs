using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Craft.ViewModels.Geometry2D.Reborn
{
    public class GeometryViewModel : INotifyPropertyChanged
    {
        private ViewState _viewState;
        private System.Windows.Point? _cursorWorldPosition;

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

        public ObservableCollection<LineModel> Lines { get; }
            = new ObservableCollection<LineModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
