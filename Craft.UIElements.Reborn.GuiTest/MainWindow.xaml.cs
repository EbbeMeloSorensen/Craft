using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Craft.UIElements.Reborn.GuiTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private static bool IsValidNumberReplacement(TextBox input, string text) =>
            MainWindowViewModel.IsValidLabelNumber(
                input.Text.Remove(input.SelectionStart, input.SelectionLength).Insert(input.SelectionStart, text));

        private void DrawingLabelNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValidNumberReplacement((TextBox)sender, e.Text);
        }

        private void DrawingLabelNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void DrawingLabelNumber_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.UnicodeText) ||
                e.DataObject.GetData(DataFormats.UnicodeText) is not string text ||
                !IsValidNumberReplacement((TextBox)sender, text))
                e.CancelCommand();
        }

        private void MainWindow_OnLoaded(
            object sender,
            RoutedEventArgs e)
        {
            ViewModel.OnLoaded();
        }

        private void MainWindow_KeyDown(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat || Keyboard.FocusedElement is TextBoxBase || Keyboard.FocusedElement is PasswordBox)
            {
                return;
            }

            ViewModel.HandleKeyEvent(e.Key);
        }
    }
}