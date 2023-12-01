namespace ArduinoVoltageReader.Interfaces
{
    public interface IMicrocontroller
    {
        float GetSingleAI();
        string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMilliseconds);
        int[] GetContinuousAI(int sampleIntervalInMilliseconds);
    }
}
