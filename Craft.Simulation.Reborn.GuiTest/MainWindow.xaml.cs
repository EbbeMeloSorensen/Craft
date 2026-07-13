using System.ComponentModel;
using System.Windows;
using Craft.Simulation.Engine;

namespace Craft.Simulation.Reborn.GuiTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel { get { return DataContext as MainWindowViewModel; } }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

        private void MainWindow_OnClosing(
            object sender,
            CancelEventArgs e)
        {
            ViewModel.SimulationLaboratoryViewModel.HandleClosing();

            //if (ViewModel.CurrentViewModel is SimulationLaboratoryViewModel simulationLaboratoryViewModel)
            //{
            //    simulationLaboratoryViewModel.HandleClosing();
            //}
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
                case System.Windows.Input.Key.Up:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.UpArrow, KeyEventType.KeyPressed);
                    break;
                case System.Windows.Input.Key.Down:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.DownArrow, KeyEventType.KeyPressed);
                    break;
                case System.Windows.Input.Key.Left:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.LeftArrow, KeyEventType.KeyPressed);
                    break;
                case System.Windows.Input.Key.Right:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.RightArrow, KeyEventType.KeyPressed);
                    break;
                case System.Windows.Input.Key.Space:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.Space, KeyEventType.KeyPressed);
                    break;
            }

            //if (ViewModel.CurrentViewModel is SimulationLaboratoryViewModel simulationLaboratoryViewModel)
            //{
            //    switch (e.Key)
            //    {
            //        case System.Windows.Input.Key.Up:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.UpArrow, KeyEventType.KeyPressed);
            //            break;
            //        case System.Windows.Input.Key.Down:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.DownArrow, KeyEventType.KeyPressed);
            //            break;
            //        case System.Windows.Input.Key.Left:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.LeftArrow, KeyEventType.KeyPressed);
            //            break;
            //        case System.Windows.Input.Key.Right:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.RightArrow, KeyEventType.KeyPressed);
            //            break;
            //        case System.Windows.Input.Key.Space:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.Space, KeyEventType.KeyPressed);
            //            break;
            //    }
            //}
        }

        private void MainWindow_KeyUp(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Up:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.UpArrow, KeyEventType.KeyReleased);
                    break;
                case System.Windows.Input.Key.Down:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.DownArrow, KeyEventType.KeyReleased);
                    break;
                case System.Windows.Input.Key.Left:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.LeftArrow, KeyEventType.KeyReleased);
                    break;
                case System.Windows.Input.Key.Right:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.RightArrow, KeyEventType.KeyReleased);
                    break;
                case System.Windows.Input.Key.Space:
                    ViewModel.SimulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.Space, KeyEventType.KeyReleased);
                    break;
            }

            //if (ViewModel.CurrentViewModel is SimulationLaboratoryViewModel simulationLaboratoryViewModel)
            //{
            //    switch (e.Key)
            //    {
            //        case System.Windows.Input.Key.Up:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.UpArrow, KeyEventType.KeyReleased);
            //            break;
            //        case System.Windows.Input.Key.Down:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.DownArrow, KeyEventType.KeyReleased);
            //            break;
            //        case System.Windows.Input.Key.Left:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.LeftArrow, KeyEventType.KeyReleased);
            //            break;
            //        case System.Windows.Input.Key.Right:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.RightArrow, KeyEventType.KeyReleased);
            //            break;
            //        case System.Windows.Input.Key.Space:
            //            simulationLaboratoryViewModel.Engine.HandleKeyEvent(KeyboardKey.Space, KeyEventType.KeyReleased);
            //            break;
            //    }
            //}
        }
    }
}