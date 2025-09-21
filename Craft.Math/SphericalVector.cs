namespace Craft.Math
{
    public class SphericalVector
    {
        public double RadialDistance { get; }
        public double PolarAngle { get; }
        public double AzimuthalAngle { get; }

        public SphericalVector(
            double radialDistance, 
            double polarAngle, 
            double azimuthalAngle)
        {
            RadialDistance = radialDistance;
            PolarAngle = polarAngle;
            AzimuthalAngle = azimuthalAngle;
        }

        public Vector3D AsVector3D()
        {
            return new Vector3D(
                RadialDistance * System.Math.Sin(PolarAngle) * System.Math.Cos(AzimuthalAngle),
                RadialDistance * System.Math.Sin(PolarAngle) * System.Math.Sin(AzimuthalAngle),
                RadialDistance * System.Math.Cos(PolarAngle));
        }

        public override string ToString()
        {
            return $"(Radial Distance: {RadialDistance,-10:F3}, Polar Angle: {PolarAngle,-10:F3}, Azimuthal Angle: {AzimuthalAngle,-10:F3})";
        }
    }
}
