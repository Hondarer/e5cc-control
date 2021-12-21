// https://www.makcraft.com/blog/meditation/2014/05/05/creation-and-display-chart-image-in-wpf/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace e5ccControlPanel.Views.Chart
{
    abstract class ChartBase
    {
        private const double SCALE_TICK_LEN = 5;

        public ChartBase(int canvasWidth, int canvasHeight)
        {
            ZoomRatioX = ZoomRatioY = 1;
            IsMaxAxesFromDatas = true;
            MinAxisX = MinAxisY = 0;
            MaxAxisX = MaxAxisY = 1;
            AxisXNicValStart = AxisYNickValStart = 1;
            AxisXNickValIncrements = AxisYNickValIncrements = 1;
            AxisXScaleStart = AxisYScaleStart = 1;
            AxisXScaleIncrements = AxisYScaleIncrements = 1;
            FrameColor = Colors.Gray;

            var dpi = (Double)DeviceHelper.PixelsPerInch(Orientation.Horizontal);
            _canvas = new RenderTargetBitmap(canvasWidth, canvasHeight, dpi, dpi, PixelFormats.Default);
        }

        private RenderTargetBitmap _canvas;
        /// <summary>
        /// グラフを描くカンバスを取得します。
        /// </summary>
        public RenderTargetBitmap Canvas
        {
            get { return _canvas; }
        }

        /// <summary>
        /// グラフのX軸拡大縮小の比率
        /// </summary>
        public double ZoomRatioX { get; set; }

        /// <summary>
        /// グラフのY軸拡大縮小の比率
        /// </summary>
        public double ZoomRatioY { get; set; }

        /// <summary>
        /// X 軸長の最大値からの余白値
        /// </summary>
        public int MarginX { get; set; }

        /// <summary>
        /// Y 軸長の最大値からの余白値
        /// </summary>
        public int MarginY { get; set; }

        /// <summary>
        /// グラフ表示場所の移動量(X 座標)
        /// </summary>
        public int TranslateX { get; set; }
        /// <summary>
        /// グラフ表示場所の移動量(Y 座標)
        /// </summary>
        public int TranslateY { get; set; }

        /// <summary>
        /// X 軸, Y 軸の最大値をデータ値から設定するか
        /// </summary>
        public bool IsMaxAxesFromDatas { get; set; }

        /// <summary>
        /// Y 軸の最小値
        /// </summary>
        public int MinAxisY { get; set; }

        /// <summary>
        /// Y 軸の最大値
        /// </summary>
        public int MaxAxisY { get; set; }

        /// <summary>
        /// X 軸の最小値
        /// </summary>
        public int MinAxisX { get; set; }

        /// <summary>
        /// X 軸の最大値
        /// </summary>
        public int MaxAxisX { get; set; }

        /// <summary>
        /// Y 軸の標題
        /// </summary>
        public string AxisYHeader { get; set; }
        /// <summary>
        /// X 軸の標題
        /// </summary>
        public string AxisXHeader { get; set; }

        /// <summary>
        /// Y 軸値表示を行うか
        /// </summary>
        public bool IsAxisYNickVal { get; set; }
        /// <summary>
        /// Y 軸値表示の開始値
        /// </summary>
        public int AxisYNickValStart { get; set; }
        /// <summary>
        /// Y 軸値表示の増分値
        /// </summary>
        public int AxisYNickValIncrements { get; set; }
        /// <summary>
        /// X 軸値表示を行うか
        /// </summary>
        public bool IsAxisXNickVal { get; set; }
        /// <summary>
        /// X 軸値表示の開始値
        /// </summary>
        public int AxisXNicValStart { get; set; }
        /// <summary>
        /// X 軸値表示の増分値
        /// </summary>
        public int AxisXNickValIncrements { get; set; }

        /// <summary>
        /// X 軸目盛り表示を行うか
        /// </summary>
        public bool IsAxisXScale { get; set; }

        /// <summary>
        /// X 軸目盛り表示の最大化
        /// </summary>
        public bool IsAxisXScaleLenMax { get; set; }

        /// <summary>
        /// X 軸目盛り表示の開始値
        /// </summary>
        public int AxisXScaleStart { get; set; }
        /// <summary>
        /// X 軸目盛り表示の増分値
        /// </summary>
        public int AxisXScaleIncrements { get; set; }

        /// <summary>
        /// Y 軸目盛り表示を行うか
        /// </summary>
        public bool IsAxisYScale { get; set; }

        /// <summary>
        /// Y 軸目盛り表示の最大化
        /// </summary>
        public bool IsAxisYScaleLenMax { get; set; }

        /// <summary>
        /// Y 軸目盛り表示の開始値
        /// </summary>
        public int AxisYScaleStart { get; set; }
        /// <summary>
        /// Y 軸目盛り表示の増分値
        /// </summary>
        public int AxisYScaleIncrements { get; set; }

        /// <summary>
        /// 枠線と目盛りの色
        /// </summary>
        public Color FrameColor { get; set; }

        /// <summary>
        /// グラフを描画します。
        /// </summary>
        /// <param name="datas"></param>
        public void DrawGraph(ChartData[] chartDatas)
        {
            var seriesGeometries = new Geometry[chartDatas.Length];
            for (var i = 0; i < chartDatas.Length; ++i)
            {
                seriesGeometries[i] = getSeriesGeometry(chartDatas[i].Datas);
            }

            if (IsMaxAxesFromDatas)
            {
                Rect rect;
                int currentX;
                int currentY;
                for (var i = 0; i < chartDatas.Length; ++i)
                {
                    rect = seriesGeometries[i].GetRenderBounds(new Pen(new SolidColorBrush(Colors.Black), 2));
                    currentX = (int)(rect.Width / ZoomRatioX) + MarginX;
                    currentY = (int)(rect.Height / ZoomRatioY) + MarginY;
                    MaxAxisX = MaxAxisX < currentX ? currentX : MaxAxisX;
                    MaxAxisY = MaxAxisY < currentY ? currentY : MaxAxisY;
                }
            }
            var frameGeometry = getFrameGeometry();

            var dv = new DrawingVisual();
            var dc = dv.RenderOpen();
            // グラフ描画位置の移動量をセット
            var transTrans = new TranslateTransform(TranslateX, TranslateY);
            dc.PushTransform(transTrans);
            // Y 軸の中心で反転をセット(左上原点補正)
            var scaleTrans = new ScaleTransform();
            scaleTrans.CenterY = MaxAxisY * ZoomRatioY / 2;
            scaleTrans.ScaleY = -1;
            dc.PushTransform(scaleTrans);
            // フレームを描画
            dc.DrawGeometry(null, new Pen(new SolidColorBrush(FrameColor), 1), frameGeometry);

            for (var i = 0; i < chartDatas.Length; ++i)
            {
                // 塗りつぶし設定
                SolidColorBrush fillBrush = chartDatas[i].IsFill ? new SolidColorBrush(chartDatas[i].FillColor) : null;
                // グラフを描画
                dc.DrawGeometry(fillBrush, new Pen(new SolidColorBrush(chartDatas[i].BorderColor), 2), seriesGeometries[i]);
            }
            // 反転設定を解除
            dc.Pop();
            // X軸, Y軸の標題
            Axes(dc);
            dc.Pop();
            dc.Close();
            // カンバスへ描画
            ((RenderTargetBitmap)_canvas).Clear();
            ((RenderTargetBitmap)_canvas).Render(dv);
        }

        /// <summary>
        /// カンバスの描画をリセット
        /// </summary>
        public void Reset()
        {
            _canvas.Clear();
        }

        /// <summary>
        /// データ値のグラフの幾何学図形を取得します。
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        protected abstract Geometry getSeriesGeometry(object[] datas);

        private Geometry getFrameGeometry()
        {
            var zoomedMinX = MinAxisX * ZoomRatioX;
            var zoomedMaxX = MaxAxisX * ZoomRatioX;
            var zoomedMinY = MinAxisY * ZoomRatioY;
            var zoomedMaxY = MaxAxisY * ZoomRatioY;
            PathFigure frameFigure;
            frameFigure = new PathFigure();
            var points = new Point[4] { new Point(zoomedMaxX, zoomedMinY), new Point(zoomedMaxX, zoomedMaxY),
                new Point(zoomedMinX, zoomedMaxY), new Point(zoomedMinX, zoomedMinY) };
            frameFigure.StartPoint = new Point(zoomedMinX, zoomedMinY);
            LineSegment lineSeg;
            var segCollect = new PathSegmentCollection();
            for (var i = 0; i < points.Length; ++i)
            {
                lineSeg = new LineSegment();
                lineSeg.Point = points[i];
                lineSeg.IsStroked = true;
                segCollect.Add(lineSeg);
            }
            frameFigure.Segments = segCollect;
            var figCollect = new PathFigureCollection();
            figCollect.Add(frameFigure);

            // Y axis scale
            if (IsAxisYScale)
            {
                drawScale(figCollect, Axis.Y, new Int32Rect((int)MinAxisX, (int)MinAxisY, (int)MaxAxisX, (int)MaxAxisY), AxisYScaleStart, AxisYScaleIncrements);
            }
            // X axis scale
            if (IsAxisXScale)
            {
                drawScale(figCollect, Axis.X, new Int32Rect((int)MinAxisX, (int)MinAxisY, (int)MaxAxisX, (int)MaxAxisY), AxisXScaleStart, AxisXScaleIncrements);
            }

            var frameGeometry = new PathGeometry();
            frameGeometry.Figures = figCollect;

            return frameGeometry;
        }

        // 目盛り描画
        private void drawScale(PathFigureCollection figCollect, Axis axis, Int32Rect rect, int start, int increments)
        {
            PathFigure scaleFigure;
            LineSegment scaleSeg;
            PathSegmentCollection scaleCollect;
            var minX = rect.X * ZoomRatioX;
            var minY = rect.Y * ZoomRatioY;
            var max = axis == Axis.X ? rect.Width : rect.Height;
            var zoomed = 0d;

            // 0軸
            //scaleFigure = new PathFigure();
            //scaleFigure.StartPoint = axis == Axis.Y ? new Point(0, minY) : new Point(minX, 0);
            //scaleSeg = axis == Axis.Y ? new LineSegment(new Point(0, max * ZoomRatioY), true) :
            //    new LineSegment(new Point(max * ZoomRatioX, 0), true);
            //scaleCollect = new PathSegmentCollection();
            //scaleCollect.Add(scaleSeg);
            //scaleFigure.Segments = scaleCollect;
            //figCollect.Add(scaleFigure);

            // 目盛りの長さ
            var tickLen = axis == Axis.Y ? minX + SCALE_TICK_LEN : minY + SCALE_TICK_LEN;
            if (axis == Axis.Y && IsAxisYScaleLenMax)
            {
                tickLen = rect.Width * ZoomRatioX;
            }
            else if (axis == Axis.X && IsAxisXScaleLenMax)
            {
                tickLen = rect.Height * ZoomRatioY;
            }
            for (var i = start; i < max; i += increments)
            {
                scaleFigure = new PathFigure();
                zoomed = axis == Axis.Y ? i * ZoomRatioY : i * ZoomRatioX;
                scaleFigure.StartPoint = axis == Axis.Y ? new Point(minX, zoomed) : new Point(zoomed, minY);
                scaleSeg = axis == Axis.Y ? new LineSegment(new Point(tickLen, zoomed), true) :
                    new LineSegment(new Point(zoomed, tickLen), true);
                scaleCollect = new PathSegmentCollection();
                scaleCollect.Add(scaleSeg);
                scaleFigure.Segments = scaleCollect;
                figCollect.Add(scaleFigure);
            }
        }

        private void Axes(DrawingContext dc)
        {
            FormattedText text;
            // Y軸目盛値表示
            Point point;
            double minX = 0;
            if (IsAxisYNickVal)
            {
                foreach (var n in getNicks(AxisYNickValStart, AxisYNickValIncrements, MaxAxisY))
                {
                    text = getFormattedText(n.ToString());
                    point = getNickPoint(text, n, MaxAxisY, Axis.Y, ZoomRatioX, ZoomRatioY);
                    point.X += MinAxisX * ZoomRatioX;
                    dc.DrawText(text, point);
                    if (point.X < minX) minX = point.X;
                }
            }
            // Y軸標題
            text = getFormattedText(AxisYHeader);
            point = getPoint(text, minX, ZoomRatioY);
            // Y軸標題の中心で反時計回りに90度回転させる
            dc.PushTransform(new RotateTransform(-90, point.X + text.Width / 2, point.Y + text.Height / 2));
            dc.DrawText(text, point);
            dc.Pop();
            // X軸目盛値表示
            if (IsAxisXNickVal)
            {
                foreach (var n in getNicks(AxisXNicValStart, AxisXNickValIncrements, MaxAxisX))
                {
                    text = getFormattedText(n.ToString());
                    point = getNickPoint(text, n, MaxAxisY, Axis.X, ZoomRatioX, ZoomRatioY);
                    point.Y -= MinAxisY * ZoomRatioY;
                    dc.DrawText(text, point);
                }
            }
            // X軸標題
            text = getFormattedText(AxisXHeader);
            dc.DrawText(text, getPoint(text, 1));
        }

        private static int[] getNicks(int start, int increment, int maxValue)
        {
            var temp = new List<int>();
            for (var i = start; i < maxValue; i += increment)
            {
                temp.Add(i);
            }

            return temp.ToArray();
        }

        private static FormattedText getFormattedText(string text)
        {
            var yAxisText = new FormattedText(text,
                new CultureInfo("ja-jp"),
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("HGSMinchoE"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                24, new SolidColorBrush(Colors.Orange));

            return yAxisText;
        }

        /// <summary>
        /// 目盛り値表示の座標計算
        /// </summary>
        /// <param name="text"></param>
        /// <param name="nickNum">表示する目盛り値(軸上の位置)</param>
        /// <param name="height"></param>
        /// <param name="axis"></param>
        /// <param name="zoomRatioX"></param>
        /// <param name="zoomRatioY"></param>
        /// <returns></returns>
        private static Point getNickPoint(FormattedText text, int nickNum, int height, Axis axis, double zoomRatioX, double zoomRatioY)
        {
            var result = new Point();
            if (axis == Axis.X)
            {   // X軸の場合
                result.X = nickNum * zoomRatioX - text.Width / 2;
                result.Y = height * zoomRatioY;
            }
            else
            {   // Y軸の場合
                result.X = -text.Width - 10; // 10 は位置の微調整値
                result.Y = -nickNum * zoomRatioY + height * zoomRatioY - text.Height / 2 - 2; // 2 は位置の微調整値
            }

            return result;
        }

        /// <summary>
        /// X軸標題の座標計算
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineNo">表示行位置</param>
        /// <returns></returns>
        private Point getPoint(FormattedText text, int lineNo)
        {
            var x = (MinAxisX * ZoomRatioX + MaxAxisX * ZoomRatioX) / 2d - text.Width / 2d;
            var y = MaxAxisY * ZoomRatioY - MinAxisY * ZoomRatioY + text.Height * lineNo + 10; // 10 は位置の微調整値

            return new Point(x, y);
        }

        /// <summary>
        /// Y軸標題の座標計算
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <param name="nickX">Y軸目盛り値表示の最小X座標</param>
        /// <returns></returns>
        private Point getPoint(FormattedText text, double nickX, double zoomRatioY)
        {
            var x = nickX - text.Width;
            var y = (MaxAxisY * zoomRatioY - MinAxisY * ZoomRatioY) / 2d - text.Height / 2d;

            return new Point(x, y);
        }

        private enum Axis
        {
            Y,
            X
        }
    }
}
