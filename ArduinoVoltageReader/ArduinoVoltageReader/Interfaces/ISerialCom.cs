namespace ArduinoVoltageReader.Interfaces
{
    public interface ISerialCom
    {
        public string DeviceType { get; }

        public void Initialize();
        public string WriteSerial(string message);
        public string ReadSerial();
    }
}
