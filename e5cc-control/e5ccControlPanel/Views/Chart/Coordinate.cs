// https://www.makcraft.com/blog/meditation/2014/05/05/creation-and-display-chart-image-in-wpf/

namespace e5ccControlPanel.Views.Chart
{
    internal class Coordinate
    {
        public Coordinate() { }
        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X 座標
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// Y 座標
        /// </summary>
        public double Y { get; set; }
    }
}
