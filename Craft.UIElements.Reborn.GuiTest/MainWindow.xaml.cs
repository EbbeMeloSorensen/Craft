using System.Windows;

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
            if (e.IsRepeat)
            {
                return;
            }

            switch (e.Key)
            {
                case System.Windows.Input.Key.Delete:
                    var a = 0;
                    break;
                case System.Windows.Input.Key.Enter:
                    var b = 0;
                    break;
                case System.Windows.Input.Key.Escape:
                    var c = 0;
                    break;
            }
        }
    }
}