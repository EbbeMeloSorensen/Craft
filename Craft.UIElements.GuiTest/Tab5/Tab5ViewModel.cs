using System.Windows.Media;
using Craft.Utils;
using Craft.ViewModels.Graph;
using GalaSoft.MvvmLight;

namespace Craft.UIElements.GuiTest.Tab5
{
    public class Tab5ViewModel : ViewModelBase
    {
        private bool _allowMovingVertices;
        //private int _selectedVertexCanvasPositionLeft;
        //private int _selectedVertexCanvasPositionTop;
        private PointD _selectedVertexCanvasPosition;

        private readonly Brush _northAmericaBrush = new SolidColorBrush(Colors.Yellow);
        private readonly Brush _southAmericaBrush = new SolidColorBrush(Colors.DarkOrange);
        private readonly Brush _europeBrush = new SolidColorBrush(Colors.CornflowerBlue);
        private readonly Brush _africaBrush = new SolidColorBrush(Colors.Red);
        private readonly Brush _asiaBrush = new SolidColorBrush(Colors.MediumSeaGreen);
        private readonly Brush _oceaniaBrush = new SolidColorBrush(Colors.MediumPurple);

        public bool AllowMovingVertices
        {
            get => _allowMovingVertices;
            set
            {
                _allowMovingVertices = value;
                GraphViewModel.AllowMovingVertices = _allowMovingVertices;
                RaisePropertyChanged();
            }
        }

        public PointD SelectedVertexCanvasPosition
        {
            get => _selectedVertexCanvasPosition;
            set
            {
                _selectedVertexCanvasPosition = value;
                RaisePropertyChanged();
            }
        }

        public GraphViewModel GraphViewModel { get; }

        public Tab5ViewModel()
        {
            var graph = GraphGenerator.GenerateGraphOfRiskTerritories();

            GraphViewModel = new GraphViewModel(graph, 1200, 900){ AllowMovingVertices = false };
            AllowMovingVertices = GraphViewModel.AllowMovingVertices;

            StyleGraph();

            GraphViewModel.VertexClicked += (s, e) =>
            {
                var point = GraphViewModel.PointViewModels[e.ElementId].Point;
                SelectedVertexCanvasPosition = new PointD(point.X - 16, point.Y - 16);
            };
        }

        private void StyleGraph()
        {
            // North America
            GraphViewModel.PlacePoint(0, new PointD(50, 50));
            GraphViewModel.PlacePoint(1, new PointD(150, 100));
            GraphViewModel.PlacePoint(2, new PointD(300, 100));
            GraphViewModel.PlacePoint(3, new PointD(100, 150));
            GraphViewModel.PlacePoint(4, new PointD(200, 150));
            GraphViewModel.PlacePoint(5, new PointD(300, 150));
            GraphViewModel.PlacePoint(6, new PointD(100, 200));
            GraphViewModel.PlacePoint(7, new PointD(250, 200));
            GraphViewModel.PlacePoint(8, new PointD(175, 250));

            // South America
            GraphViewModel.PlacePoint(9, new PointD(175, 350));
            GraphViewModel.PlacePoint(10, new PointD(125, 400));
            GraphViewModel.PlacePoint(11, new PointD(175, 450));
            GraphViewModel.PlacePoint(12, new PointD(225, 400));

            // Europe
            GraphViewModel.PlacePoint(13, new PointD(400, 100));
            GraphViewModel.PlacePoint(14, new PointD(500, 100));
            GraphViewModel.PlacePoint(15, new PointD(450, 150));
            GraphViewModel.PlacePoint(16, new PointD(500, 200));
            GraphViewModel.PlacePoint(17, new PointD(550, 150));
            GraphViewModel.PlacePoint(18, new PointD(450, 250));
            GraphViewModel.PlacePoint(19, new PointD(550, 250));

            // Africa
            GraphViewModel.PlacePoint(20, new PointD(450, 350));
            GraphViewModel.PlacePoint(21, new PointD(550, 350));
            GraphViewModel.PlacePoint(22, new PointD(600, 400));
            GraphViewModel.PlacePoint(23, new PointD(500, 400));
            GraphViewModel.PlacePoint(24, new PointD(500, 450));
            GraphViewModel.PlacePoint(25, new PointD(600, 450));

            // Asia
            GraphViewModel.PlacePoint(26, new PointD(825, 150));
            GraphViewModel.PlacePoint(27, new PointD(725, 150));
            GraphViewModel.PlacePoint(28, new PointD(875, 100));
            GraphViewModel.PlacePoint(29, new PointD(1025, 50));
            GraphViewModel.PlacePoint(30, new PointD(925, 150));
            GraphViewModel.PlacePoint(31, new PointD(725, 225));
            GraphViewModel.PlacePoint(32, new PointD(925, 225));
            GraphViewModel.PlacePoint(33, new PointD(1025, 225));
            GraphViewModel.PlacePoint(34, new PointD(825, 225));
            GraphViewModel.PlacePoint(35, new PointD(675, 300));
            GraphViewModel.PlacePoint(36, new PointD(775, 300));
            GraphViewModel.PlacePoint(37, new PointD(875, 300));

            // Oceania
            GraphViewModel.PlacePoint(38, new PointD(875, 400));
            GraphViewModel.PlacePoint(39, new PointD(975, 400));
            GraphViewModel.PlacePoint(40, new PointD(875, 450));
            GraphViewModel.PlacePoint(41, new PointD(975, 450));

            Enumerable
                .Range(0, 9)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _northAmericaBrush, ""));

            Enumerable
                .Range(9, 4)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _southAmericaBrush, ""));

            Enumerable
                .Range(13, 7)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _europeBrush, ""));

            Enumerable
                .Range(20, 6)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _africaBrush, ""));

            Enumerable
                .Range(26, 12)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _asiaBrush, ""));

            Enumerable
                .Range(38, 4)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _oceaniaBrush, ""));
        }
    }
}
