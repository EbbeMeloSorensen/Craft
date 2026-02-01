using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using Craft.Algorithms.GuiTest.Common;
using Craft.DataStructures.Graph;
using Craft.Utils;
using Craft.Math;
using Craft.ViewModels.Common;
using Craft.ViewModels.Geometry2D.Scrolling;

namespace Craft.Algorithms.GuiTest.Tab3
{
    // Line of Sight
    public class Tab3ViewModel : ImageEditorViewModel
    {
        private Point2D _viewPoint;
        private GraphAdjacencyList<Point2DVertex, EmptyEdge> _graph;
        private ObservableCollection<LineSegment2D> _wallLines;
        private ObservableCollection<LineSegment2D> _triangleLines;
        private ObservableCollection<Triangle2DViewModel> _triangleViewModels;
        private ObservableCollection<PointViewModel> _vertexViewModels;
        private bool _pointWasClicked;
        private PointViewModel _activeViewModel;
        private int _indexOfActiveVertex;
        private Point2D _initialVertex;
        private RelayCommand _recalculateCommand;

        public ObservableCollection<LineSegment2D> WallLines
        {
            get
            {
                return _wallLines;
            }
            set
            {
                _wallLines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<LineSegment2D> TriangleLines
        {
            get
            {
                return _triangleLines;
            }
            set
            {
                _triangleLines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PointViewModel> VertexViewModels
        {
            get
            {
                return _vertexViewModels;
            }
            set
            {
                _vertexViewModels = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Triangle2DViewModel> TriangleViewModels
        {
            get
            {
                return _triangleViewModels;
            }
            set
            {
                _triangleViewModels = value;
                RaisePropertyChanged();
            }
        }

        // We use this to communicate to the view if a point was clicked when a mouse down event occured
        public bool PointWasClicked
        {
            get
            {
                var result = _pointWasClicked;
                _pointWasClicked = false;
                return result;
            }
        }

        public RelayCommand RecalculateCommand
        {
            get
            {
                return _recalculateCommand ?? (_recalculateCommand = new RelayCommand(UpdateVisibleRegion));
            }
        }

        public Tab3ViewModel(
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            var vertices = new List<Point2DVertex>
            {
                new Point2DVertex(10, 10),
                new Point2DVertex(310, 10),
                new Point2DVertex(310, 310),
                new Point2DVertex(10, 310),
                new Point2DVertex(140, 30),
                new Point2DVertex(260, 30),
                new Point2DVertex(260, 100),
                new Point2DVertex(140, 100),
                new Point2DVertex(30, 200),
                new Point2DVertex(30, 270),
                new Point2DVertex(100, 270),
                new Point2DVertex(100, 200)
            };

            _viewPoint = new Point2D(155, 155);

            // Den her skal vi bruge til at identificere nabo-punkterne
            _graph = new GraphAdjacencyList<Point2DVertex, EmptyEdge>(vertices, false);
            _graph.AddEdge(new EmptyEdge(0, 1));
            _graph.AddEdge(new EmptyEdge(1, 2));
            _graph.AddEdge(new EmptyEdge(2, 3));
            _graph.AddEdge(new EmptyEdge(3, 0));
            _graph.AddEdge(new EmptyEdge(4, 5));
            _graph.AddEdge(new EmptyEdge(5, 6));
            _graph.AddEdge(new EmptyEdge(6, 7));
            _graph.AddEdge(new EmptyEdge(7, 4));
            _graph.AddEdge(new EmptyEdge(8, 9));
            _graph.AddEdge(new EmptyEdge(9, 10));
            _graph.AddEdge(new EmptyEdge(10, 11));
            _graph.AddEdge(new EmptyEdge(11, 8));

            _vertexViewModels = new ObservableCollection<PointViewModel>();

            var vertexIndex = 0;
            foreach (var vertex in _graph.Vertices)
            {
                var pointViewModel = new PointViewModel(vertex.AsPointD(), vertexIndex, 10);

                pointViewModel.ElementClicked += ElementViewModelElementClicked;

                _vertexViewModels.Add(pointViewModel);

                vertexIndex++;
            }

            var viewPointViewModel = new PointViewModel(_viewPoint.AsPointD(), vertexIndex, 20);

            viewPointViewModel.ElementClicked += ElementViewModelElementClicked;
            _vertexViewModels.Add(viewPointViewModel);

            UpdateLines();
            UpdateVisibleRegion();
        }

        public void MovePoint(PointD offset)
        {
            var point = new Point2D(
                _initialVertex.X + offset.X,
                _initialVertex.Y + offset.Y);

            _activeViewModel.Point = point.AsPointD();

            if (_indexOfActiveVertex < _graph.VertexCount)
            {
                _graph.UpdateVertex(_indexOfActiveVertex, new Point2DVertex(point.X, point.Y));
            }
            else
            {
                _viewPoint = new Point2D(point.X, point.Y);
            }

            UpdateLines();
            UpdateVisibleRegion();
        }

        private void ElementViewModelElementClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pointWasClicked = true;
            _indexOfActiveVertex = e.ElementId;
            _activeViewModel = VertexViewModels[_indexOfActiveVertex];

            _initialVertex = new Point2D(
                _activeViewModel.Point.X,
                _activeViewModel.Point.Y);
        }

        private void UpdateLines()
        {
            // Det her er lige rigeligt grimt..

            WallLines = new ObservableCollection<LineSegment2D>(
                _graph.Edges.Select(e => new LineSegment2D(
                    new Point2D(_graph.Vertices[e.VertexId1].X, _graph.Vertices[e.VertexId1].Y),
                    new Point2D(_graph.Vertices[e.VertexId2].X, _graph.Vertices[e.VertexId2].Y))));
        }

        private void UpdateVisibleRegion()
        {
            var triangles = VisibleRegion.IdentifyVisibleRegion(_graph, _viewPoint);

            TriangleLines = new ObservableCollection<LineSegment2D>();
            TriangleViewModels = new ObservableCollection<Triangle2DViewModel>();

            triangles.ForEach(t =>
            {
                TriangleLines.Add(new LineSegment2D(t.Point1, t.Point2));
                TriangleLines.Add(new LineSegment2D(t.Point2, t.Point3));
                TriangleLines.Add(new LineSegment2D(t.Point3, t.Point1));

                TriangleViewModels.Add(new Triangle2DViewModel(new Triangle2D(
                    t.Point1,
                    t.Point2,
                    t.Point3)));
            });
        }
    }
}
