using e5ccControlPanel.Commands;
using e5ccControlPanel.Views.Chart;
using libE5cc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace e5ccControlPanel.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        internal E5ccDriver driver = new E5ccDriver();

        private DispatcherTimer refreshTimer = new DispatcherTimer();
        private DispatcherTimer countdownTimer = new DispatcherTimer();

        private double _pv;

        public double PV
        {
            get => _pv;
            set => SetProperty(ref _pv, value);
        }

        private double _setPoint;

        public double SetPoint
        {
            get => _setPoint;
            set => SetProperty(ref _setPoint, value);
        }

        private double _mv_heating;

        public double MV_Heating
        {
            get => _mv_heating;
            set => SetProperty(ref _mv_heating, value);
        }

        #region Status

        private bool _controlOutput_Heating;

        public bool ControlOutput_Heating
        {
            get => _controlOutput_Heating;
            set => SetProperty(ref _controlOutput_Heating, value);
        }

        private bool _alarm1;

        public bool Alarm1
        {
            get => _alarm1;
            set => SetProperty(ref _alarm1, value);
        }

        private bool _alarm2;

        public bool Alarm2
        {
            get => _alarm2;
            set => SetProperty(ref _alarm2, value);
        }

        private bool _ramMode;

        public bool RamMode
        {
            get => _ramMode;
            set => SetProperty(ref _ramMode, value);
        }

        private bool _notEqualRamAndVolatileMemory;

        public bool NotEqualRamAndVolatileMemory
        {
            get => _notEqualRamAndVolatileMemory;
            set => SetProperty(ref _notEqualRamAndVolatileMemory, value);
        }

        private bool _communicationWriting;

        public bool CommunicationWriting
        {
            get => _communicationWriting;
            set => SetProperty(ref _communicationWriting, value);
        }

        #endregion

        private TimeSpan? _targetTime;

        public TimeSpan? TargetTime
        {
            get => _targetTime;
            set => SetProperty(ref _targetTime, value);
        }

        private DateTime? _startTime;

        public DateTime? StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private TimeSpan? _elapsedTime;

        public TimeSpan? ElapsedTime
        {
            get => _elapsedTime;
            set => SetProperty(ref _elapsedTime, value);
        }

        private TimeSpan? _remainTime;

        public TimeSpan? RemainTime
        {
            get => _remainTime;
            set => SetProperty(ref _remainTime, value);
        }

        private DateTime recordedTime = DateTime.Now;

        protected class RecordData
        {
            public double PV { get; set; }

            public double SetPoint { get; set; }

            public double MV_Heating { get; set; }
        }

        private List<Tuple<DateTime, RecordData>> records = new List<Tuple<DateTime, RecordData>>();

        public MainWindowViewModel()
        {
            refreshTimer.Interval = TimeSpan.FromSeconds(1);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            countdownTimer.Interval = TimeSpan.FromMilliseconds(50);
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            DrawChart();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                ResponseBase response = driver.SendCommand(new ReadVariableMultipleCommand() { ReadStartAddress = 0x0000, NumberOfElements = 6 });
                var variables = ((ReadVariableMultipleResponse)response).Variables;

                Debug.WriteLine($"{DateTime.Now}");
                foreach (var data in variables)
                {
                    Debug.WriteLine($"0x{data.Key:x4}: {data.Value}");
                }

                PV = variables[0x0000] / 10.0D;
                MV_Heating = variables[0x0008] / 10.0D;

                ControlOutput_Heating = (variables[0x0002] & 0x00000100) != 0;
                Alarm1 = (variables[0x0002] & 0x00001000) != 0;
                Alarm2 = (variables[0x0002] & 0x00002000) != 0;
                RamMode = (variables[0x0002] & 0x00100000) != 0;
                NotEqualRamAndVolatileMemory = (variables[0x0002] & 0x00200000) != 0;
                CommunicationWriting = (variables[0x0002] & 0x02000000) != 0;

                response = driver.SendCommand(new ReadVariableMultipleCommand() { ReadStartAddress = 0x0106, NumberOfElements = 7 });
                variables = ((ReadVariableMultipleResponse)response).Variables;
                SetPoint = variables[0x0106] / 10.0D;

                DateTime nowMinute = DateTime.Now;
                nowMinute = nowMinute.AddTicks(-(nowMinute.Ticks % TimeSpan.TicksPerSecond));
                nowMinute = nowMinute.AddSeconds(-(nowMinute.Second % 10));

                if (recordedTime != nowMinute)
                {
                    recordedTime = nowMinute;

                    records.Add(new Tuple<DateTime, RecordData>(nowMinute, new RecordData() { PV = PV, SetPoint = SetPoint, MV_Heating = MV_Heating }));

                    using (StreamWriter writer = new StreamWriter("./log.txt", true, Encoding.UTF8))
                    {
                        writer.WriteLine($"{recordedTime}\t{PV}\t{SetPoint}\t{MV_Heating}");
                    }
                    DrawChart();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // 再スローはしない
            }
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (StartTime == null || TargetTime == null)
            {
                ElapsedTime = null;
                RemainTime = null;
                return;
            }

            ElapsedTime = new TimeSpan(DateTime.UtcNow.Ticks - ((DateTime)StartTime).Ticks);
            RemainTime = new TimeSpan(((TimeSpan)TargetTime).Ticks - ((TimeSpan)ElapsedTime).Ticks);
        }

        public ICommand UnboundCommand { get; } = new DelegateCommand((parameter) =>
        {
            if (parameter is MainWindowViewModel vm)
            {
                vm.StartTime = null;
                vm.TargetTime = null;

                // 設定温度変更
                try
                {
                    var response = vm.driver.SendCommand(new WriteVariableSingleCommand() { WriteVariableAddress = 0x2103, WriteData = 5000 });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // 再スローはしない
                }
            }
        });

        public ICommand TorihamuCommand { get; } = new DelegateCommand((parameter) =>
        {
            if (parameter is MainWindowViewModel vm)
            {
                vm.StartTime = null;
                vm.TargetTime = TimeSpan.FromMinutes(90);

                // 設定温度変更
                try
                {
                    var response = vm.driver.SendCommand(new WriteVariableSingleCommand() { WriteVariableAddress = 0x2103, WriteData = 600 });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // 再スローはしない
                }
            }
        });

        public ICommand OntamaCommand { get; } = new DelegateCommand((parameter) =>
        {
            if (parameter is MainWindowViewModel vm)
            {
                vm.StartTime = null;
                vm.TargetTime = TimeSpan.FromMinutes(30);

                // 設定温度変更
                try
                {
                    var response = vm.driver.SendCommand(new WriteVariableSingleCommand() { WriteVariableAddress = 0x2103, WriteData = 640 });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // 再スローはしない
                }
            }
        });

        public ICommand ConfitCommand { get; } = new DelegateCommand((parameter) =>
        {
            if (parameter is MainWindowViewModel vm)
            {
                vm.StartTime = null;
                vm.TargetTime = TimeSpan.FromMinutes(90);

                // 設定温度変更
                try
                {
                    var response = vm.driver.SendCommand(new WriteVariableSingleCommand() { WriteVariableAddress = 0x2103, WriteData = 440 });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // 再スローはしない
                }
            }
        });

        public ICommand StartCommand { get; } = new DelegateCommand((parameter) =>
        {
            if (parameter is MainWindowViewModel vm)
            {
                vm.StartTime = DateTime.UtcNow;
            }
        });

        public ICommand StopCommand { get; } = new DelegateCommand((parameter) =>
        {
            if (parameter is MainWindowViewModel vm)
            {
                vm.StartTime = null;
            }
        });

        #region chart

        // https://www.makcraft.com/blog/meditation/2014/05/05/creation-and-display-chart-image-in-wpf/

        private const int IMAGE_WIDTH = 700;
        private const int IMAGE_HEIGHT = 420;
        private LineSeriesChart _chart1;

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                SetProperty(ref _imageSource, value);
            }
        }

        private double _imageWidth;
        public double ImageWidth
        {
            get { return _imageWidth; }
            set
            {
                SetProperty(ref _imageWidth, value);
            }
        }

        private double _imageHeight;
        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                SetProperty(ref _imageHeight, value);
            }
        }

        private void DrawChart()
        {
            if (_chart1 == null)
            {
                _chart1 = new LineSeriesChart(IMAGE_WIDTH, IMAGE_HEIGHT);
                ImageSource = _chart1.Canvas;
                ImageWidth = IMAGE_WIDTH;
                ImageHeight = IMAGE_HEIGHT;

                _chart1.ZoomRatioX = 8.0;
                _chart1.ZoomRatioY = 2.0;
                _chart1.TranslateX = 590;
                _chart1.TranslateY = 10;
                _chart1.IsMaxAxesFromDatas = false;
                _chart1.MinAxisX = -60;
                _chart1.MaxAxisX = 10;
                _chart1.MinAxisY = 10;
                _chart1.MaxAxisY = 180;
                _chart1.AxisXHeader = "経過時間[分]";
                _chart1.AxisYHeader = "温度[℃]";
                _chart1.IsAxisXNickVal = true;
                _chart1.IsAxisYNickVal = true;
                _chart1.AxisXNicValStart = -60;
                _chart1.AxisYNickValStart = 20;
                _chart1.AxisXNickValIncrements = 10;
                _chart1.AxisYNickValIncrements = 20;
                _chart1.FrameColor = Colors.DarkOrange;
                _chart1.IsAxisXScale = _chart1.IsAxisYScale = true;
                _chart1.IsAxisXScaleLenMax = _chart1.IsAxisYScaleLenMax = true;
                _chart1.AxisXScaleStart = -60;
                _chart1.AxisYScaleStart = 10;
                _chart1.AxisXScaleIncrements = 10;
                _chart1.AxisYScaleIncrements = 10;
            }

            var chartData = new ChartData[] {
                new ChartData { Datas = FutureSetPointFunction(), BorderColor = Colors.DarkRed },
                new ChartData { Datas = SetPointFunction(), BorderColor = Colors.Red },
                new ChartData { Datas = PVFunction(), BorderColor = Colors.Orange }
            };

            _chart1.DrawGraph(chartData);
        }

        private Coordinate[] FutureSetPointFunction()
        {
            var result = new List<Coordinate>();

            result.Add(new Coordinate(0, SetPoint));
            result.Add(new Coordinate(10, SetPoint));

            return result.ToArray();
        }

        private Coordinate[] SetPointFunction()
        {
            var result = new List<Coordinate>();

            foreach (var record in records)
            {
                var elapsedMin = TimeSpan.FromTicks(record.Item1.Ticks - recordedTime.Ticks).TotalMinutes;
                var value = record.Item2.SetPoint;

                if (value != 500.0D)
                {
                    result.Add(new Coordinate(elapsedMin, value));
                }
            }

            return result.ToArray();
        }

        private Coordinate[] PVFunction()
        {
            var result = new List<Coordinate>();

            foreach (var record in records)
            {
                var elapsedMin = TimeSpan.FromTicks(record.Item1.Ticks - recordedTime.Ticks).TotalMinutes;
                var value = record.Item2.PV;

                result.Add(new Coordinate(elapsedMin, value));
            }

            return result.ToArray();
        }

        #endregion
    }
}
