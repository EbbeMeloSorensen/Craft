using System.Windows;
using System.Windows.Controls;

namespace Craft.ViewModels.Geometry2D.ScrollFree;

public class LabelTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(
        object item,
        DependencyObject container)
    {
        var element = container as FrameworkElement;

        return element.FindResource("Label") as DataTemplate;
    }
}