using Craft.ViewModels.Geometry2D.Reborn;
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
    }
}