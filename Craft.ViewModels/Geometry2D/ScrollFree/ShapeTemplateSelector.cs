using System.Windows;
using System.Windows.Controls;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    // Denne kan vi referere til fra en DataTemplate i en xaml fil. Den hjælper med at tildele en DataTemplate defineret i en
    // ressourcefil i xaml til en given instans af en viewmodel. Den kan passende overrides med en anden, hvis man for en given
    // applikation gerne vil operere med et andet skin.
    public class ShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            var element = container as FrameworkElement;

            return item switch
            {
                RotatableRectangleViewModel => element.FindResource("RotatableRectangle") as DataTemplate,
                RectangleViewModel => element.FindResource("Rectangle") as DataTemplate,
                RotatableEllipseViewModel => element.FindResource("RotatableEllipse") as DataTemplate,
                EllipseViewModel => element.FindResource("Ellipse") as DataTemplate,
                _ => throw new ArgumentException("item doesn't correspond to any DataTemplate")
            };
        }
    }
}
