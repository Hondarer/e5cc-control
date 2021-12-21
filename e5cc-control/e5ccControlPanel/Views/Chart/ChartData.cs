// https://www.makcraft.com/blog/meditation/2014/05/05/creation-and-display-chart-image-in-wpf/

using System.Windows.Media;

namespace e5ccControlPanel.Views.Chart
{
    internal class ChartData
    {
        public ChartData()
        {
            BorderColor = Colors.Black;
            FillColor = Colors.Gray;
        }

        /// <summary>
        /// グラフの線の色
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// グラフの塗りつぶし
        /// </summary>
        public bool IsFill { get; set; }

        /// <summary>
        /// グラフの塗りつぶし色
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// グラフ描画を描く値の配列
        /// </summary>
        public object[] Datas { get; set; }
    }
}
