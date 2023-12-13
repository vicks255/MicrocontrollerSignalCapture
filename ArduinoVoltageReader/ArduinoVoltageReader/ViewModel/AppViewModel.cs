using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using ArduinoVoltageReader.Interfaces;
using GalaSoft.MvvmLight.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace ArduinoVoltageReader.ViewModel
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public AppViewModel(IServiceProvider services)
        {
            _services = services;
            _device = _services.GetService<IDevice>();

            IsChannel1Checked = true;
            IsChannel2Checked = true;
        }

        private static  IServiceProvider? _services;
        private static  IDevice _device;

        private string _channels;

        public event PropertyChangedEventHandler PropertyChanged;


        public List<float[]> Channel1Capture { get; set; }
        public List<float[]> Channel2Capture { get; set; }
        public string SamplingRate { get; set; } = "100";
        public string SamplingWindow { get; set; } = "10";
        public string VoltageRange { get; set; } = "5";


        private bool _isChannel1Checked;
        public bool IsChannel1Checked
        {
            get => _isChannel1Checked;
            set
            {
                _isChannel1Checked = value;
                Channel1Visible = _isChannel1Checked ? Channel1Visible = Visibility.Visible : Channel1Visible = Visibility.Hidden;

                _channels = getChannels();
                OnPropertyChanged();
            }
        }

        private bool _isChannel2Checked;
        public bool IsChannel2Checked
        {
            get => _isChannel2Checked;
            set
            {
                _isChannel2Checked = value;
                Channel2Visible = _isChannel2Checked ? Channel2Visible = Visibility.Visible : Channel2Visible = Visibility.Hidden;

                _channels = getChannels();
                OnPropertyChanged();
            }
        }

        private Visibility _channel1Visible;
        public Visibility Channel1Visible
        {
            get => _channel1Visible;
            set
            {
                _channel1Visible = value;
                OnPropertyChanged();
            }
        }

        private Visibility _channel2Visible;
        public Visibility Channel2Visible
        {
            get => (_channel2Visible);
            set
            {
                _channel2Visible = value;
                OnPropertyChanged();
            }
        }


        private string _connectionStatus;
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }


        private RelayCommand _writeData;
        public RelayCommand WriteData => _writeData ?? (_writeData = new RelayCommand(WriteCSV));


        private void WriteCSV()
        {
            if(!(Channel1Capture is null))
            {
                string writeFile = $"Microseconds,Volts";
                if (_channels.Contains("1"))
                {
                    foreach (float[] measurement in Channel1Capture)
                    {
                        writeFile += $"\r\n{measurement[0]},{measurement[1]}";
                    }
                }
                if (_channels.Contains("2"))
                {
                    foreach (float[] measurement in Channel2Capture)
                    {
                        writeFile += $"\r\n{measurement[0]},{measurement[1]}";
                    }
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV File|*.csv",
                    Title = "Save Data To CSV",
                };
                saveFileDialog.ShowDialog();

                if(!string.IsNullOrEmpty(saveFileDialog.FileName))
                    File.WriteAllText(saveFileDialog.FileName, writeFile);
            }
            else
            {
                MessageBox.Show("No Data To Export", "No Data", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public GraphData ReadWindowAIVoltage()
        {
            if(int.TryParse(SamplingWindow, out int samplingWindow) && int.TryParse(SamplingRate, out int samplingRate))
            {
                string signalCapture = _device.GetWindowAI(samplingWindow, samplingRate, _channels);

                List<float[]> channel1Capture = new List<float[]>();
                List<float[]> channel2Capture = new List<float[]>();
                string[] point;

                string[] dataPoints = signalCapture.Split('\r');
                float minMicroSeconds = float.Parse(dataPoints[0].Split(",")[0]);
                float maxMicroSeconds = float.Parse(dataPoints[0].Split(",")[0]);
                float minReading = float.Parse(dataPoints[0].Split(",")[1]);
                float maxReading = float.Parse(dataPoints[0].Split(",")[1]);

                for (int index = 1; index < dataPoints.Length; index++)
                {
                    point = dataPoints[index].Trim().Split(',');

                    if (_channels.Contains("1"))
                    {
                        channel1Capture.Add(new float[2] { float.Parse(point[0]) - minMicroSeconds, float.Parse(point[1]) });
                        minMicroSeconds = float.Min(minMicroSeconds, float.Parse(point[0]));
                        maxMicroSeconds = float.Max(maxMicroSeconds, float.Parse(point[0]));
                        minReading = float.Min(minReading, float.Parse(point[1]));
                        maxReading = float.Min(maxReading, float.Parse(point[1]));
                    }
                    if (_channels.Contains("2"))
                    {
                        channel2Capture.Add(new float[2] { float.Parse(point[2]) - minMicroSeconds, float.Parse(point[3]) });
                        minMicroSeconds = float.Min(minMicroSeconds, float.Parse(point[2]));
                        maxMicroSeconds = float.Max(maxMicroSeconds, float.Parse(point[2]));
                        minReading = float.Min(minReading, float.Parse(point[3]));
                        maxReading = float.Min(maxReading, float.Parse(point[3]));
                    }
                }

                Channel1Capture = channel1Capture;
                Channel2Capture = channel2Capture;
                return new GraphData { XMax = maxMicroSeconds, YMax = maxReading, Channel1Points = channel1Capture, Channel2Points = channel2Capture };
            }
            return null;
        }


        private string getChannels()
        {
            string channels = "";
            if (IsChannel1Checked)
                channels = "1";

            if(IsChannel2Checked) 
                channels += "2";

            return channels;
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
        public List<float[]> Channel1Points { get; set; }
        public List<float[]> Channel2Points { get; set; }
    }
}
