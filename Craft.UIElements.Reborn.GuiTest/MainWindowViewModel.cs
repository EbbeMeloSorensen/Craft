using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Craft.ViewModels.Geometry2D.Reborn;

namespace Craft.UIElements.Reborn.GuiTest
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public GeometryViewModel GeometryViewModel { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            GeometryViewModel = new GeometryViewModel
            {
                LockAspectRatio = true
            };

            DrawAHouse();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void DrawAHouse()
        {
            GeometryViewModel.Lines.Add(new LineModel
            {
                P1 = new Point(0, 0),
                P2 = new Point(0, 200)
            });

            GeometryViewModel.Lines.Add(new LineModel
            {
                P1 = new Point(0, 200),
                P2 = new Point(200, 300)
            });

            GeometryViewModel.Lines.Add(new LineModel
            {
                P1 = new Point(200, 300),
                P2 = new Point(400, 200)
            });

            GeometryViewModel.Lines.Add(new LineModel
            {
                P1 = new Point(400, 200),
                P2 = new Point(400, 0)
            });

            GeometryViewModel.Lines.Add(new LineModel
            {
                P1 = new Point(400, 0),
                P2 = new Point(0, 0)
            });
        }
    }
}
