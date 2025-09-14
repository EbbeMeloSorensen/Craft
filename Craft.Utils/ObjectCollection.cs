using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Craft.Utils
{
    // The idea with this class is to wrap a sequence of objects in an object that can raise an event when the sequence is
    // replaced with an entirely new one. The ObservableCollection class notifies subscribers about changes to itself,
    // but this implies that its life cycle doesn't end

    public class ObjectCollection<T> : INotifyPropertyChanged
    {
        private IEnumerable<T> _objects;

        public IEnumerable<T> Objects
        {
            get { return _objects; }
            set
            {
                _objects = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
