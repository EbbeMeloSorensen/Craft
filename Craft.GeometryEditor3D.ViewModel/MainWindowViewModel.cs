using Craft.Math;
using GalaSoft.MvvmLight;

namespace Craft.GeometryEditor3D.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private double _cameraX;
        private double _cameraY;
        private double _cameraZ;

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public double CameraX
        {
            get { return _cameraX; }
            set
            {
                _cameraX = value;
                RaisePropertyChanged();

                GeometryEditorViewModel.CameraPosition = new Vector3D(
                    _cameraX,
                    GeometryEditorViewModel.CameraPosition.Y,
                    GeometryEditorViewModel.CameraPosition.Z);
            }
        }

        public double CameraY
        {
            get { return _cameraY; }
            set
            {
                _cameraY = value;
                RaisePropertyChanged();

                GeometryEditorViewModel.CameraPosition = new Vector3D(
                    GeometryEditorViewModel.CameraPosition.X,
                    _cameraY,
                    GeometryEditorViewModel.CameraPosition.Z);
            }
        }

        public double CameraZ
        {
            get { return _cameraZ; }
            set
            {
                _cameraZ = value;
                RaisePropertyChanged();

                GeometryEditorViewModel.CameraPosition = new Vector3D(
                    GeometryEditorViewModel.CameraPosition.X,
                    GeometryEditorViewModel.CameraPosition.Y,
                    _cameraZ);
            }
        }

        public MainWindowViewModel(GeometryEditorViewModel geometryEditorViewModel)
        {
            GeometryEditorViewModel = geometryEditorViewModel;
            CameraX = GeometryEditorViewModel.CameraPosition.X;
            CameraY = GeometryEditorViewModel.CameraPosition.Y;
            CameraZ = GeometryEditorViewModel.CameraPosition.Z;
        }
    }
}
