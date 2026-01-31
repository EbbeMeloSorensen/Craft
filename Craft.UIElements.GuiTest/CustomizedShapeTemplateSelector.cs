using System.Windows;
using System.Windows.Controls;
using Craft.UIElements.GuiTest.Tab3;
using Craft.ViewModels.Geometry2D.ScrollFree;

namespace Craft.UIElements.GuiTest
{
    // This DataTemplateSelector may be used for overriding the one in the Craft.ViewModels assembly
    public class CustomizedShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item,
            DependencyObject container)
        {
            var element = container as FrameworkElement;

            return item switch
            {
                RotatableRectangleViewModel => element.FindResource("RotatableRectangle") as DataTemplate,
                YellowBar => element.FindResource("YellowBar") as DataTemplate,
                RectangleViewModel => element.FindResource("DoorAndWindow") as DataTemplate,
                RotatableEllipseViewModel => element.FindResource("RotatableEllipse") as DataTemplate,
                EllipseViewModel => element.FindResource("Sun") as DataTemplate,
                _ => throw new ArgumentException("item doesn't correspond to any DataTemplate")
            };
        }
    }
}
