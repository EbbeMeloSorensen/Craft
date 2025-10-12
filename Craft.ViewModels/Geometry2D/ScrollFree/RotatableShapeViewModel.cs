namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public abstract class RotatableShapeViewModel : ShapeViewModel
    {
        private double _orientation;
        private System.Windows.Media.Matrix _rotationMatrix;
            
        public double Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                RaisePropertyChanged();

                UpdateRotationMatrix();
            }
        }

        public System.Windows.Media.Matrix RotationMatrix
        {
            get { return _rotationMatrix; }
            set
            {
                _rotationMatrix = value;
                RaisePropertyChanged();
            }
        }

        protected virtual void UpdateRotationMatrix()
        {
            var cosAngle = System.Math.Cos(Orientation);
            var sinAngle = System.Math.Sin(Orientation);
            var x = Width / 2;
            var y = Height / 2;

            var T1 = new System.Windows.Media.Matrix(1, 0, 0, 1, -x, -y);
            var R = new System.Windows.Media.Matrix(cosAngle, -sinAngle, sinAngle, cosAngle, 0, 0);
            var T2 = new System.Windows.Media.Matrix(1, 0, 0, 1, x, y);

            RotationMatrix = T1 * R * T2;
        }
    }
}
