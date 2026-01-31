using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using Craft.Utils;

namespace Craft.ViewModels.Common
{
    public class PixelViewModel : ViewModelBase
    {
        private int _id;
        private Pixel _pixel;
        private RelayCommand _leftClickedCommand;
        private RelayCommand _rightClickedCommand;

        public Pixel Pixel
        {
            get { return _pixel; }
            set
            {
                _pixel = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand LeftClickedCommand
        {
            get
            {
                return _leftClickedCommand ?? (_leftClickedCommand = new RelayCommand(LeftClicked));
            }
        }

        public RelayCommand RightClickedCommand
        {
            get
            {
                return _rightClickedCommand ?? (_rightClickedCommand = new RelayCommand(RightClicked));
            }
        }

        public event EventHandler<ElementClickedEventArgs> PixelLeftClicked;
        public event EventHandler<ElementClickedEventArgs> PixelRightClicked;

        public PixelViewModel(
            int id,
            Pixel pixel)
        {
            _id = id;
            _pixel = pixel;
        }

        private void LeftClicked()
        {
            OnPixelLeftClicked();
        }

        private void RightClicked()
        {
            OnPixelRightClicked();
        }

        private void OnPixelLeftClicked()
        {
            var handler = PixelLeftClicked;

            if (handler != null)
            {
                handler(this, new ElementClickedEventArgs(_id));
            }
        }

        private void OnPixelRightClicked()
        {
            var handler = PixelRightClicked;

            if (handler != null)
            {
                handler(this, new ElementClickedEventArgs(_id));
            }
        }
    }
}
