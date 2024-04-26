using ArduinoVoltageReader.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System;

namespace ArduinoVoltageReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow(AppViewModel appViewModel)
        {
            _appViewModel = appViewModel;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private AppViewModel _appViewModel;
        private int _graphWidth = 650;
        private int _graphHeight = 300;
        private bool _isDVMActive = false;
        private float _calibrationFactor = 6;


        private void GetSignalCapture(object sender, RoutedEventArgs e)
        {
            GraphData dataPoints = _appViewModel.ReadWindowAIVoltage();

            Graph.Children.Clear();
            SetGraph(int.Parse(_appViewModel.SamplingWindow), int.Parse(_appViewModel.VoltageRange));

            if (_appViewModel.IsChannel1Checked)
                DrawPoints(dataPoints.Channel1Points, new SolidColorBrush(Colors.Red));

            if (_appViewModel.IsChannel2Checked)
                DrawPoints(dataPoints.Channel2Points, new SolidColorBrush(Colors.Blue));
        }


        private void RecordDVM(object sender, RoutedEventArgs e)
        {
            _isDVMActive = !_isDVMActive;

            if (_isDVMActive)
            {
                Thread getReadings = new Thread(GetLiveData);
                getReadings.SetApartmentState(ApartmentState.STA);
                getReadings.Start();

                _appViewModel.DVMMode = "Stop DVM";
            }
            else
            {
                _appViewModel.DVMMode = "Start DVM";
            }
        }
        

        private void GetLiveData()
        {
            float[] dvmReading;
            Stopwatch sw = new Stopwatch();

            this.Dispatcher.Invoke((Action)(() => {
                Graph.Children.Clear();
                SetGraph(int.Parse(_appViewModel.SamplingWindow), int.Parse(_appViewModel.VoltageRange)); ;
            }));

            sw.Start();
            while (_isDVMActive)
            {
                dvmReading = _appViewModel.GetSingleReading();
                if(sw.Elapsed.TotalMilliseconds > 10000)
                {
                    this.Dispatcher.Invoke((Action)(() => {
                        Graph.Children.Clear();
                        SetGraph(int.Parse(_appViewModel.SamplingWindow), int.Parse(_appViewModel.VoltageRange)); ;

                        sw.Restart();
                    }));
                }


                if (_appViewModel.IsChannel1Checked)
                {
                    _appViewModel.Channel1SingleReading = System.Math.Round(dvmReading[0] * _calibrationFactor, 2).ToString();

                    this.Dispatcher.Invoke((Action)(() => {
                        DrawSinglePoint(dvmReading[0] * _calibrationFactor, sw.Elapsed.TotalMilliseconds, new SolidColorBrush(Colors.Red));
                    }));
                }

                if (_appViewModel.IsChannel2Checked)
                {
                    _appViewModel.Channel2SingleReading = $"{System.Math.Round(dvmReading[1] * _calibrationFactor, 2).ToString()}";

                    this.Dispatcher.Invoke((Action)(() => {
                        DrawSinglePoint(dvmReading[1] * _calibrationFactor, sw.Elapsed.TotalMilliseconds, new SolidColorBrush(Colors.Blue));
                    }));
                }
            }
            sw.Stop();
        }


        private void DrawSinglePoint(float dataPoint, double timeStamp, SolidColorBrush solidColorBrush)
        {
            double xValue;
            float yValue;

            xValue = (timeStamp / (float.Parse(_appViewModel.SamplingWindow) * 1000)) * _graphWidth;
            yValue = (dataPoint / float.Parse(_appViewModel.VoltageRange)) * _graphHeight;

            Ellipse ellipse = new Ellipse
            {
                Height = 5,
                Width = 5,
                Fill = solidColorBrush,
                ToolTip = $"Time: {timeStamp} ms\r\nVoltage: {dataPoint} V",
            };

            Graph.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, xValue - 2.5);
            Canvas.SetBottom(ellipse, yValue - 2.5);
        }


        private void DrawPoints(List<float[]> dataPoints, SolidColorBrush solidColorBrush)
        {
            float xValue;
            float yValue;
            float X1 = 0;
            float Y1 = 0;
            bool firstPoint = true;

            foreach (float[] shape in dataPoints)
            {
                xValue = (shape[0] * _calibrationFactor / (float.Parse(_appViewModel.SamplingWindow) * 1000)) * _graphWidth;
                yValue = (shape[1] * _calibrationFactor / float.Parse(_appViewModel.VoltageRange)) * _graphHeight;

                Ellipse channel1Ellipse = new Ellipse
                {
                    Height = 5,
                    Width = 5,
                    Fill = solidColorBrush,
                    ToolTip = $"Time: {shape[0]} us\r\nVoltage: {shape[1]} V",
                    Visibility = _appViewModel.Channel1Visible
                };
                Graph.Children.Add(channel1Ellipse);
                Canvas.SetLeft(channel1Ellipse, xValue - 2.5);
                Canvas.SetBottom(channel1Ellipse, yValue - 2.5);

                if (!firstPoint)
                {
                    Line channel1Line = new Line
                    {
                        X1 = X1,
                        Y1 = -Y1,
                        X2 = xValue,
                        Y2 = -yValue,
                        StrokeThickness = 1,
                        Stroke = solidColorBrush,
                        Visibility = _appViewModel.Channel1Visible
                    };
                    Graph.Children.Add(channel1Line);
                    Canvas.SetLeft(channel1Line, 0);
                    Canvas.SetBottom(channel1Line, 0);
                }
                firstPoint = false;
                X1 = xValue;
                Y1 = yValue;
            }


            firstPoint = true;
            foreach (float[] shape in dataPoints)
            {
                xValue = (shape[0] * _calibrationFactor / (float.Parse(_appViewModel.SamplingWindow) * 1000)) * _graphWidth;
                yValue = (shape[1] * _calibrationFactor / float.Parse(_appViewModel.VoltageRange)) * _graphHeight;

                Ellipse channel1Ellipse = new Ellipse
                {
                    Height = 5,
                    Width = 5,
                    Fill = solidColorBrush,
                    ToolTip = $"Time: {shape[0]} us\r\nVoltage: {shape[1]} V",
                    Visibility = _appViewModel.Channel2Visible
                };
                Graph.Children.Add(channel1Ellipse);
                Canvas.SetLeft(channel1Ellipse, xValue - 2.5);
                Canvas.SetBottom(channel1Ellipse, yValue - 2.5);

                if (!firstPoint)
                {
                    Line channel1Line = new Line
                    {
                        X1 = X1,
                        Y1 = -Y1,
                        X2 = xValue,
                        Y2 = -yValue,
                        StrokeThickness = 1,
                        Stroke = solidColorBrush,
                        Visibility = _appViewModel.Channel2Visible
                    };
                    Graph.Children.Add(channel1Line);
                    Canvas.SetLeft(channel1Line, 0);
                    Canvas.SetBottom(channel1Line, 0);
                }
                firstPoint = false;
                X1 = xValue;
                Y1 = yValue;
            }
        }


        private void SetGraph(int xInterval, int yInterval)
        {
            for(int xLine = 0; xLine <= xInterval; xLine++)
            {
                Line newLine = new Line
                {
                    X1 = (_graphWidth / xInterval) * xLine,
                    Y1 = 0,
                    X2 = (_graphWidth / xInterval) * xLine,
                    Y2 = _graphHeight,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Gray)
                };
                Graph.Children.Add (newLine);
            }

            for (int yLine = 0; yLine <= yInterval; yLine++)
            {
                Line newLine = new Line
                {
                    X1 = 0,
                    Y1 = (_graphHeight / yInterval) * yLine,
                    X2 = _graphWidth,
                    Y2 = (_graphHeight / yInterval) * yLine,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Gray)
                };
                Graph.Children.Add(newLine);
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string control = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(control));
        }
    }
}
