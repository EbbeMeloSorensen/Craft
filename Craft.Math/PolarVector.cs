namespace Craft.Math
{
    public class PolarVector
    {
        public double Length { get; }
        public double Angle { get; }

        public PolarVector(
            double length,
            double angle)
        {
            Length = length;
            Angle = angle;
        }
    }
}
