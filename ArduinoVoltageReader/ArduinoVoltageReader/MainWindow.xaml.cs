using ArduinoVoltageReader.ViewModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.Generic;

namespace ArduinoVoltageReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(AppViewModel appViewModel)
        {
            _appViewModel = appViewModel;
            InitializeComponent();
        }

        private AppViewModel _appViewModel;
        private int _graphWidth = 650;
        private int _graphHeight = 300;
        private float _xMax = 10000;
        private float _yMax = 5;


        private void GetSignalCapture(object sender, RoutedEventArgs e)
        {
            GraphData dataPoints = _appViewModel.ReadWindowAIVoltage();

            _ = float.TryParse(_appViewModel.VoltageRange, out float yScale) ? _yMax = yScale : _yMax = 5;
            Graph.Children.Clear();
            SetGraph(10, 10);

            if (_appViewModel.IsChannel1Checked)
                DrawPoints(dataPoints.Channel1Points, new SolidColorBrush(Colors.Red));

            if (_appViewModel.IsChannel2Checked)
                DrawPoints(dataPoints.Channel2Points, new SolidColorBrush(Colors.Blue));
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
                xValue = (shape[0] / _xMax) * _graphWidth;
                yValue = (shape[1] / _yMax) * _graphHeight;

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
                xValue = (shape[0] / _xMax) * _graphWidth;
                yValue = (shape[1] / _yMax) * _graphHeight;

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
    }
}
