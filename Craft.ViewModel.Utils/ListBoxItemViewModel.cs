using GalaSoft.MvvmLight;

namespace Craft.ViewModel.Utils
{
    public class ListBoxItemViewModel<T> : ViewModelBase
    {
        private T _object;
        private bool _isSelected;

        public T Object
        {
            get { return _object; }
            set
            {
                _object = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }

        public ListBoxItemViewModel(T @object)
        {
            _object = @object;
        }
    }
}
