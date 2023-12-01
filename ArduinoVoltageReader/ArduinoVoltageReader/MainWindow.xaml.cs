﻿using ArduinoVoltageReader.Interfaces;
using ArduinoVoltageReader.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;

namespace ArduinoVoltageReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow(IServiceProvider services)
        {
            _services = services;
            _appViewModel = new AppViewModel(_services);
            _capturedData = new List<float[]>();
            InitializeComponent();
        }


        private IServiceProvider _services;
        private AppViewModel _appViewModel;
        private List<float[]> _capturedData;
        private int _graphWidth = 650;
        private int _graphHeight = 300;


        private void GetSignalCapture(object sender, RoutedEventArgs e)
        {
            float xValue;
            float yValue;
            float X1 = 0;
            float Y1 = 0;
            GraphData dataPoints = _appViewModel.ReadWindowAIVoltage();

            dataPoints.YMax = 1;
            Graph.Children.Clear();
            SetGraph(10, 4);            

            foreach(float[] shape in dataPoints.DataPoints)
            {
                xValue = (shape[0] / dataPoints.XMax) * _graphWidth;
                yValue = (shape[1] / dataPoints.YMax) * _graphHeight;

                Ellipse newEllipse = new Ellipse
                {
                    Height = 10,
                    Width = 10,
                    Fill = new SolidColorBrush(Colors.Red),
                    ToolTip = $"Time: {shape[0]} us\r\nVoltage: {shape[1]} V"
                };

                Line newLine = new Line
                {
                    X1 = X1,
                    Y1 = -Y1,
                    X2 = xValue,
                    Y2 = -yValue,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Red)
                };

                X1 = xValue;
                Y1 = yValue;

                Graph.Children.Add(newEllipse);
                Graph.Children.Add(newLine);

                Canvas.SetLeft(newEllipse, xValue - 5);
                Canvas.SetBottom(newEllipse, yValue - 5);

                Canvas.SetLeft(newLine, 0);
                Canvas.SetBottom(newLine, 0);
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
