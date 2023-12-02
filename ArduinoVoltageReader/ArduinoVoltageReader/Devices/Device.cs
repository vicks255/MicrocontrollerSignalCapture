using ArduinoVoltageReader.DeviceServiceRegistration;
using ArduinoVoltageReader.Interfaces;
using System;
using System.Windows;

namespace ArduinoVoltageReader.Devices
{
    public class Device : IMicrocontroller
    {
        public Device(IServiceProvider services,  string deviceType)
        {
            switch (deviceType)
            {
                case "PortentaH7":
                    _controller = (IMicrocontroller) new PortentaH7(services);
                    break;

                case "TivaC_123":
                    _controller = (IMicrocontroller) new TivaC_123(services);
                    break;

                default:
                    MessageBox.Show("No Microcontroller Detected:\r\nLoading simulated device", "No Microcontroller Detected", MessageBoxButton.OK);
                    _controller = (IMicrocontroller)new MockDevice();
                    break;
            }
        }

        private IMicrocontroller _controller;

        public string ControllerModel => _controller.GetType().Name;

        public int[] GetContinuousAI(int sampleIntervalInMilliseconds) => _controller.GetContinuousAI(sampleIntervalInMilliseconds);
        public float GetSingleAI() => _controller.GetSingleAI();
        public string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMilliseconds) => _controller.GetWindowAI(windowInMilliseconds, sampleIntervalInMilliseconds);
    }
}
