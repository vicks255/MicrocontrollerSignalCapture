namespace ArduinoVoltageReader.Interfaces
{
    public interface IMicrocontroller
    {
        float GetSingleAI();
        string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMicroseconds);
        int[] GetContinuousAI(int sampleIntervalInMicroseconds);
    }
}
