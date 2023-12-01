using ArduinoVoltageReader.Devices;
using ArduinoVoltageReader.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ArduinoVoltageReader.DeviceServiceRegistration
{
    public class DeviceServiceRegistration : IDevice
    {
        public DeviceServiceRegistration(IServiceProvider service)
        {
            _services = service;
            ControllerModel = service.GetService<ISerialCom>().DeviceType;
        }

        private IServiceProvider _services;
        private static IMicrocontroller _controller;

        public string ControllerModel { get; }

        public void Initialize()
        {
            _controller = new Device(_services, ControllerModel);
        }


        public int[] GetContinuousAI(int sampleIntervalInMilliseconds)
        {
            return _controller.GetContinuousAI(sampleIntervalInMilliseconds);
        }

        public float GetSingleAI()
        {
            return _controller.GetSingleAI();
        }

        public string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMilliseconds)
        {
            return _controller.GetWindowAI(windowInMilliseconds, sampleIntervalInMilliseconds);
        }
    }
}
