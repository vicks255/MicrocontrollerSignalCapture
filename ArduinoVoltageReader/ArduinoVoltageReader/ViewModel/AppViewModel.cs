using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using ArduinoVoltageReader.Interfaces;
using ArduinoVoltageReader.Views;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.DependencyInjection;

namespace ArduinoVoltageReader.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public AppViewModel(IServiceProvider services)
        {
            _services = services;
            _device = _services.GetService<IDevice>();
        }

        private static  IServiceProvider? _services;
        private static  IDevice _device;
        private string _readVoltage;


        public event PropertyChangedEventHandler PropertyChanged;


        public List<float[]> CapturedData { get; set; }
        public string SamplingRate { get; set; } = "";
        public string SamplingWindow { get; set; } = "";
        public string VoltageRange { get; set; } = "";


        public string ReadVoltage
        {
            get { return _readVoltage; }
            set
            {
                _readVoltage = value;
                OnPropertyChanged();
            }
        }

        private string _readVoltageWindow;
        public string ReadVoltageWindow
        {
            get { return _readVoltageWindow; }
            set
            {
                _readVoltageWindow = value;
                OnPropertyChanged();
            }
        }

        private string _connectionStatus;
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GraphView> _canvasGraph;
        public ObservableCollection<GraphView> CanvasGraph
        {
            get { return _canvasGraph; }
            set
            {
                _canvasGraph = value;
                OnPropertyChanged();
            }
        }


        private RelayCommand _writeData;
        public RelayCommand WriteData => _writeData ?? (_writeData = new RelayCommand(WriteCSV));


        private void WriteCSV()
        {
            if(!(CapturedData is null))
            {
                string writeFile = $"Microseconds,Volts";
                foreach (float[] measurement in CapturedData)
                {
                    writeFile += $"\r\n{measurement[0]},{measurement[1]}";
                }
                File.WriteAllText(@"C:\DataCapture.csv", writeFile);
            }
            else
            {
                MessageBox.Show("No Data To Export", "No Data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public GraphData ReadWindowAIVoltage()
        {
            string signalCapture = _device.GetWindowAI(5, 0);

            List<float[]> dataList = new List<float[]>();
            string[] point;

            string[] dataPoints = signalCapture.Split('\r');
            float minMicros = float.Parse(dataPoints[0].Split(",")[0]);
            float maxMicros = float.Parse(dataPoints[0].Split(",")[0]);
            float minReading = float.Parse(dataPoints[0].Split(",")[1]);
            float maxReading = float.Parse(dataPoints[0].Split(",")[1]);

            for (int index = 1; index < dataPoints.Length; index++)
            {
                point = dataPoints[index].Split(',');
                dataList.Add(new float[2] { float.Parse(point[0]) - minMicros, float.Parse(point[1]) });

                if (float.Parse(point[0]) < minMicros) { minMicros = float.Parse(point[0]); }
                if (float.Parse(point[0]) > maxMicros) { maxMicros = float.Parse(point[0]); }

                if (float.Parse(point[1]) < minReading) { minReading = float.Parse(point[1]); }
                if (float.Parse(point[1]) > maxReading) { maxReading = float.Parse(point[1]); }
            }

            CapturedData = dataList;
            return new GraphData {XMax = maxMicros, YMax = maxReading, DataPoints = dataList };
        }


        protected void OnPropertyChanged([CallerMemberName] string control = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(control));
        }
    }


    public class GraphData
    {
        public float XMax { get; set; }
        public float YMax { get; set; }
        public List<float[]> DataPoints { get; set; }
    }
}
