using ArduinoVoltageReader.Interfaces;
using System;
using System.IO.Ports;


namespace ArduinoVoltageReader.Services
{
    public class SerialCommunication : ISerialCom
    {
        public SerialCommunication()
        {
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.WriteTimeout = 2000;
            _serialPort.ReadTimeout = 5000;
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;            
        }


        private SerialPort _serialPort = new SerialPort();


        public bool IsSerialPortError {get; set;}
        public string DeviceType { get; set;}        


        public void Initialize()
        {
            string comPortName = GetComPort();
            if (comPortName.Contains("ERROR"))
            {
                IsSerialPortError = true;
            }
            else
            {
                IsSerialPortError = false;
                _serialPort.PortName = comPortName;
                _serialPort.Open();
            }
        }

        public string WriteSerial(string message)
        {
            if(!IsSerialPortError)
            {
                _serialPort.Write(message);
                return ReadSerial();
            }
            return "ERROR";
        }

        public string ReadSerial()
        {
            if(!IsSerialPortError)
            {
                return _serialPort.ReadLine();                
            }
            return "";
        }

        private string GetComPort()
        {
            string[] openComPorts = SerialPort.GetPortNames();
            string arduinoPort = "ERROR";
            string received;

            try
            {
                foreach (string port in openComPorts)
                {
                    _serialPort.PortName = port;

                    if(!_serialPort.IsOpen) 
                    { 
                        _serialPort.Open(); 
                    }       

                    _serialPort.WriteLine("Model");
                    received = _serialPort.ReadLine();
                    switch (received.Trim())
                    {
                        case "PortentaH7":
                            DeviceType = "PortentaH7";
                            arduinoPort = port;
                            break;

                        case "TivaC_123":
                            DeviceType = "TivaC_123";
                            arduinoPort = port;
                            break;
                    }
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                DeviceType = "ERROR";
                return $"ERROR: {ex.Message}";
            }
            return arduinoPort;
        }
    }
}
