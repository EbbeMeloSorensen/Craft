namespace Craft.UIElements.Geometry2D.Reborn;

public static class TimeCoordinates
{
    public static readonly DateTime Epoch =
        new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long ToWorldTicks(
        DateTime t)
        => (t - Epoch).Ticks;

    public static DateTime ToDateTime(
        long ticks)
        => Epoch.AddTicks(ticks);
}