namespace Craft.ViewModels.Geometry2D.Reborn;

public interface IFrameAware
{
    void OnFrame(TimeSpan time, double dt);
}

