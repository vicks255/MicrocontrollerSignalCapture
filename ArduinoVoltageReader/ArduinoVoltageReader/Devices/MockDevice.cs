using ArduinoVoltageReader.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArduinoVoltageReader.Devices
{
    public class MockDevice : IMicrocontroller
    {
        public int[] GetContinuousAI(int sampleIntervalInMilliseconds)
        {
            throw new NotImplementedException();
        }

        public float[] GetSingleAI()
        {
            throw new NotImplementedException();
        }

        public string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMicroseconds, string channels)
        {
            string simulatedSignal = "";
            int numberOfPoints = (windowInMilliseconds * 1000) / sampleIntervalInMicroseconds;
            double angle;

            for(int point = 0; point <= numberOfPoints; point++)
            {
                angle = (2 * Math.PI * point) / numberOfPoints;
                simulatedSignal += $"\r\n{point * sampleIntervalInMicroseconds}," +
                                   $"{2.55 + (1.5 * Math.Sin(angle))}," +
                                   $"{point * sampleIntervalInMicroseconds}," +
                                   $"{2.55 + (1.5 * Math.Cos(angle))}";
            }
            return simulatedSignal.Trim();
        }
    }
}
