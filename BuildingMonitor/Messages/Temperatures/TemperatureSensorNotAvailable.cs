namespace BuildingMonitor.Messages.Temperatures
{
    public sealed class TemperatureSensorNotAvailable : ITemperatureQueryReading
    {
        public static TemperatureSensorNotAvailable Instance { get; } = new TemperatureSensorNotAvailable();
        private TemperatureSensorNotAvailable() { }
    }
}
