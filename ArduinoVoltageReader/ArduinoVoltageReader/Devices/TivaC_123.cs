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

        public string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMicroSeconds)
        {
            // Analog pin reads 0-5 volts from 0-1023
            string readings;
            string[] response = new string[3];

            // analog pin requires 100us to read.
            if (sampleIntervalInMicroSeconds < 100)
            {
                sampleIntervalInMicroSeconds = 0;
            }
            else
            {
                sampleIntervalInMicroSeconds -= 100;
            }

            response[0] = _serial.WriteSerial("window");
            response[1] = _serial.WriteSerial(windowInMilliseconds.ToString());
            response[2] = _serial.WriteSerial(sampleIntervalInMicroSeconds.ToString());

            Thread.Sleep(windowInMilliseconds + 2500);
            readings = _serial.ReadSerial().Trim();

            string[] dataPoints = readings.Split('\r');
            string calculatedReadings = "";
            if(dataPoints.Length > 0)
            {
                string[] dataPoint = dataPoints[0].Split(',');
                long startTime = long.Parse(dataPoint[0]);
                calculatedReadings = $"0,{dataPoint[1]}";

                for(int index = 1; index < dataPoints.Length; index++)
                {
                    dataPoint = dataPoints[index].Split(",");
                    calculatedReadings += $"\r\n{long.Parse(dataPoint[0]) - startTime},{int.Parse(dataPoint[1]) * 0.48876}";
                }
            }
            return calculatedReadings;
        }

        public int[] GetContinuousAI(int sampleIntervalInMilliseconds)
        {
            throw new NotImplementedException();
        }
    }
}
