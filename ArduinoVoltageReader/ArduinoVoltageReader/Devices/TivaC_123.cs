using ArduinoVoltageReader.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace ArduinoVoltageReader.DeviceServiceRegistration
{
    public class TivaC_123 : IMicrocontroller
    {
        public TivaC_123(IServiceProvider service)
        {
            _services = service;
            _serial = _services.GetService<ISerialCom>();
        }

        private IServiceProvider _services;
        private readonly ISerialCom _serial;

        public float GetSingleAI()
        {
            throw new NotImplementedException();
        }

        public int[] GetContinuousAI(int sampleIntervalInMilliseconds)
        {
            throw new NotImplementedException();
        }

        public string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMicroSeconds, string channels)
        {
            string readings;
            string[] response = new string[4];
            long[] time = new long[2];
            double[] reading = new double[2];
            double voltsToCountsRatio = 5.0 / 1023;

            // analog pin requires 100us to read.
            // if two channels are being requested, 200us will be required to read.
            int readTime = 100;
            if (channels.Equals("12"))
                readTime = 200;

            if (sampleIntervalInMicroSeconds < readTime)
                sampleIntervalInMicroSeconds = 0;
            else
                sampleIntervalInMicroSeconds -= readTime;

            response[0] = _serial.WriteSerial("window");
            response[1] = _serial.WriteSerial(windowInMilliseconds.ToString());
            response[2] = _serial.WriteSerial(sampleIntervalInMicroSeconds.ToString());
            response[3] = _serial.WriteSerial(channels);

            Thread.Sleep(windowInMilliseconds + 2500);
            readings = _serial.ReadSerial().Trim();

            string[] dataPoints = readings.Split('\r');
            string calculatedReadings = "";
            if (dataPoints.Length > 0)
            {
                string[] dataPoint = dataPoints[0].Split(',');
                long channel1StartTime = long.Parse(dataPoint[0]);
                long channel2StartTime = long.Parse(dataPoint[2]);
                calculatedReadings = $"0,{dataPoint[1]}";

                for (int index = 1; index < dataPoints.Length; index++)
                {
                    dataPoint = dataPoints[index].Split(",");
                    time[0] = long.Parse(dataPoint[0]) - channel1StartTime;
                    reading[0] = double.Parse(dataPoint[1]) * voltsToCountsRatio;
                    time[1] = long.Parse(dataPoint[2]) - channel2StartTime;
                    reading[1] = double.Parse(dataPoint[3]) * voltsToCountsRatio;

                    calculatedReadings += $"\r\n{time[0]},{reading[0]},{time[1]},{reading[1]}";
                }
            }
            return calculatedReadings.Trim();
        }
    }
}
