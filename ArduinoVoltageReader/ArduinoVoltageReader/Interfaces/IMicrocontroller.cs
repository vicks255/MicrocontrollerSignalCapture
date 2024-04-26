namespace ArduinoVoltageReader.Interfaces
{
    public interface IMicrocontroller
    {
        float[] GetSingleAI();
        string GetWindowAI(int windowInMilliseconds, int sampleIntervalInMicroseconds, string channels);
        int[] GetContinuousAI(int sampleIntervalInMicroseconds);
    }
}
