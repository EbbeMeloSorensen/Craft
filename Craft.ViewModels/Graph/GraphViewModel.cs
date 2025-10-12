using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Craft.Utils;
using Craft.DataStructures.Graph;
using Craft.ViewModels.Common;
using Craft.ViewModels.Geometry2D.Scrolling;
using System;

namespace Craft.ViewModels.Graph
{
    public class GraphViewModel : ImageEditorViewModel
    {
        private List<PointD> _points;
        private ObservableCollection<LineSegmentD> _lines;
        private ObservableCollection<PointViewModel> _pointViewModels;
        private bool _pointWasClicked;
        private PointViewModel _activeViewModel;
        private int _indexOfActivePoint;
        private PointD _initialPoint; // At start of drag
        private Brush _defaultVertexBrush = new SolidColorBrush(Colors.LightGray);

        private IGraph<LabelledVertex, EmptyEdge> _graph;

        public bool AllowMovingVertices { get; set; }

        public event EventHandler<ElementClickedEventArgs> VertexClicked;

        public ObservableCollection<LineSegmentD> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PointViewModel> PointViewModels
        {
            get
            {
                return _pointViewModels;
            }
            set
            {
                _pointViewModels = value;
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

        public GraphViewModel(
            IGraph<LabelledVertex, EmptyEdge> graph,
            int initialImageWidth,
            int initialImageHeight) : base(initialImageWidth, initialImageHeight)
        {
            AllowMovingVertices = true;
            ScrollableOffset = new PointD(0, 0);
            ScrollOffset = new PointD(0, 0);

            _graph = graph;

            _points = new List<PointD>();
            _pointViewModels = new ObservableCollection<PointViewModel>();

            var pointIndex = 0;

            _graph.Vertices.ForEach(_ =>
            {
                var point = new PointD(300, 300);
                _points.Add(point);

                var pointViewModel = new PointViewModel(point, pointIndex++, 30);

                pointViewModel.ElementClicked += ElementViewModelElementClicked;
                pointViewModel.Label = _.Label;
                pointViewModel.Brush = _defaultVertexBrush;

                _pointViewModels.Add(pointViewModel);
            });

            UpdateLines();
        }

        public void MovePoint(
            PointD offset)
        {
            var point = new PointD(
                _initialPoint.X + offset.X,
                _initialPoint.Y + offset.Y);

            _activeViewModel.Point = point;
            _points[_indexOfActivePoint] = point;

            UpdateLines();
        }

        public void PlacePoint(
            int pointIndex,
            PointD position)
        {
            _points[pointIndex] = position;
            PointViewModels[pointIndex].Point = position;
            UpdateLines();
        }

        public void StylePoint(
            int pointIndex,
            Brush brush,
            string label)
        {
            var pointViewModel = PointViewModels[pointIndex];
            pointViewModel.Brush = brush;
            pointViewModel.Label = label;
        }

        // Handler for when a PointViewModel is clicked
        private void ElementViewModelElementClicked(
            object sender,
            ElementClickedEventArgs e)
        {
            _pointWasClicked = true;
            _indexOfActivePoint = e.ElementId;
            _activeViewModel = PointViewModels[_indexOfActivePoint];
            _initialPoint = _activeViewModel.Point;

            OnVertexClicked(e.ElementId);
        }

        // For raising an event regarding a vertex being clicked
        private void OnVertexClicked(
            int vertexId)
        {
            var handler = VertexClicked;

            if (handler != null)
            {
                handler(this, new ElementClickedEventArgs(vertexId));
            }
        }

        private void UpdateLines()
        {
            Lines = new ObservableCollection<LineSegmentD>(
                _graph.Edges.Select(_ => new LineSegmentD(_points[_.VertexId1], _points[_.VertexId2])));
        }
    }
}
