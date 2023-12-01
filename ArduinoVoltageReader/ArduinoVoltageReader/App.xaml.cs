using ArduinoVoltageReader.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Microsoft.Extensions.Hosting;
using ArduinoVoltageReader.Services;
using ArduinoVoltageReader.ViewModel;
using System;

namespace ArduinoVoltageReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost? Host;
        private IServiceProvider _services;

        protected override void OnStartup(StartupEventArgs e)
        {
            Host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ISerialCom, SerialCommunication>()
                            .AddSingleton<IConfiguration, ConfigurationState>()
                            .AddSingleton<IDevice, DeviceServiceRegistration.DeviceServiceRegistration>()
                            .AddTransient<AppViewModel>();
                })
                .Build();
            Host.Start();

            _services = Host.Services;            
            _services.GetService<ISerialCom>().Initialize();
            _services.GetService<IDevice>().Initialize();

            string connectedDevice = "Connection Error";
            if (!string.IsNullOrEmpty(_services.GetService<ISerialCom>().DeviceType))
            {
                connectedDevice = _services.GetService<ISerialCom>().DeviceType;
            }

            
            AppViewModel appViewModel = new AppViewModel(_services);
            appViewModel.ConnectionStatus = $"Device Connection Status: {connectedDevice}";

            MainWindow = new MainWindow(_services);
            MainWindow.DataContext = appViewModel;
            MainWindow.Show();
        }        
    }
}
