using System.Windows;
using System.Windows.Controls;
using Craft.Utils;

namespace Craft.UI.Utils
{
    public static class Helpers
    {
        #region Attached Property facilitating scrolling by dragging
        public static PointD GetScrollOffset(DependencyObject obj)
        {
            return (PointD)obj.GetValue(ScrollOffsetProperty);
        }

        public static void SetScrollOffset(DependencyObject obj, PointD value)
        {
            obj.SetValue(ScrollOffsetProperty, value);
        }

        public static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.RegisterAttached("ScrollOffset", typeof(PointD), typeof(Helpers), new PropertyMetadata(new PointD(), ScrollOffsetPropertyChanged));

        private static void ScrollOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;

            if (scrollViewer != null && e.NewValue != null && e.NewValue != e.OldValue)
            {
                var newScrollOffset = (PointD)e.NewValue;

                scrollViewer.ScrollToHorizontalOffset(newScrollOffset.X);
                scrollViewer.ScrollToVerticalOffset(newScrollOffset.Y);
            }
        }
        #endregion

        #region Attached Property for facilitating automatic scrolling to bottom
        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(Helpers), new PropertyMetadata(false, AutoScrollPropertyChanged));

        private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;

            if (scrollViewer != null && (bool)e.NewValue)
            {
                scrollViewer.ScrollToBottom();
            }
        }
        #endregion
    }
}
