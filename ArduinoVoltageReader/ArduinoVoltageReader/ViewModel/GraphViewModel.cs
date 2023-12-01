using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ArduinoVoltageReader.ViewModel
{
    public class GraphViewModel
    {
        public Canvas ShowGraph(float xMax, float yMax, List<float[]> points)
        {
            Canvas canvas = new Canvas();

            float xValue;
            float yValue;

            float X1 = 0;
            float Y1 = 0;

            foreach (var point in points)
            {
                xValue = (point[0] / xMax);
                yValue = (point[1] / yMax);

                Ellipse newEllipse = new Ellipse
                {
                    Height = 5,
                    Width = 5,
                    Fill = new SolidColorBrush(Colors.White),
                    ToolTip = $"Time: {xValue} us\r\nVoltage: {yValue} V"
                };

                Line newLine = new Line
                {
                    X1 = X1,
                    Y1 = -Y1,
                    X2 = xValue,
                    Y2 = -yValue,
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(Colors.White)
                };

                X1 = xValue;
                Y1 = yValue;
            }
            return canvas;
        }
    }
}
