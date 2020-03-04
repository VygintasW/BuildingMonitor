namespace BuildingMonitor.Messages.Temperatures
{
    public sealed class TemperatureAvailable : ITemperatureQueryReading
    {
        public TemperatureAvailable(double temperature)
        {
            Temperature = temperature;
        }

        public double Temperature { get; }
    }
}
