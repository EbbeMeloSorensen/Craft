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
            ViewModel.SimulationLaboratoryViewModel.CaptureKeyboard = CaptureKeyboard;
            Deactivated += (_, _) => ViewModel.SimulationLaboratoryViewModel.KeyPresses.Clear();
        }

        private void MainWindow_OnClosing(
            object sender,
            CancelEventArgs e)
        {
            ViewModel.SimulationLaboratoryViewModel.HandleClosing();
        }

        private KeyboardState CaptureKeyboard()
        {
            if (!IsActive) return new KeyboardState();
            return new KeyboardState
            {
                LeftArrowDown = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Left),
                RightArrowDown = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Right),
                UpArrowDown = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Up),
                DownArrowDown = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Down),
                SpaceDown = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.Space)
            };
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var simulation = ViewModel.SimulationLaboratoryViewModel;
            if (e.IsRepeat || !IsActive || !simulation.Engine.AnimationRunning) return;
            KeyboardKey? key = e.Key switch
            {
                System.Windows.Input.Key.Left => KeyboardKey.LeftArrow,
                System.Windows.Input.Key.Right => KeyboardKey.RightArrow,
                System.Windows.Input.Key.Up => KeyboardKey.UpArrow,
                System.Windows.Input.Key.Down => KeyboardKey.DownArrow,
                System.Windows.Input.Key.Space => KeyboardKey.Space,
                _ => null
            };
            if (key.HasValue) simulation.KeyPresses.Enqueue(key.Value);
        }
    }
}
