// https://www.makcraft.com/blog/meditation/2014/05/05/creation-and-display-chart-image-in-wpf/

using System;
using System.Windows;
using System.Windows.Media;

namespace e5ccControlPanel.Views.Chart
{
    class LineSeriesChart : ChartBase
    {
        public LineSeriesChart(int canvasWidth, int canvasHeight) : base(canvasWidth, canvasHeight) { }

        protected override Geometry getSeriesGeometry(object[] vals)
        {
            if (vals.GetType() != typeof(Coordinate[]))
            {
                throw new ArgumentException("引数の型が Coordinate[] ではありません。");
            }

            if (vals.Length == 0)
            {
                return new PathGeometry();
            }

            PathFigure figure;
            figure = new PathFigure();
            figure.StartPoint = new Point(((Coordinate)vals[0]).X * ZoomRatioX, ((Coordinate)vals[0]).Y * ZoomRatioY);
            LineSegment lineSeg;
            var segCollect = new PathSegmentCollection();
            Coordinate item;
            for (var i = 1; i < vals.Length; ++i)
            {
                lineSeg = new LineSegment();
                item = (Coordinate)vals[i];
                lineSeg.Point = new Point(item.X * ZoomRatioX, item.Y * ZoomRatioY);
                lineSeg.IsStroked = true;
                segCollect.Add(lineSeg);
            }
            figure.Segments = segCollect;

            var figCollect = new PathFigureCollection();
            figCollect.Add(figure);

            var seriesGeometry = new PathGeometry();
            seriesGeometry.Figures = figCollect;

            return seriesGeometry;
        }
    }
}
