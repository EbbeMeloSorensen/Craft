using Craft.ViewModels.Geometry2D.ScrollFree;

namespace Craft.ViewModels.Simulation;

public class TaggedEllipseViewModel : EllipseViewModel
{
    private string _tag;

    public string Tag
    {
        get => _tag;
        set
        {
            _tag = value;
            RaisePropertyChanged();
        }
    }
}

