namespace ArduinoVoltageReader.Interfaces
{
    public interface IDevice
    {
        string ControllerModel { get; }

        void Initialize();

        int[] GetContinuousAI(int sampleIntervalInMilliseconds);
        float GetSingleAI();
        string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMicroSeconds);
    }
}
