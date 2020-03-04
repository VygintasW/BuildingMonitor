namespace BuildingMonitor.Messages.Temperatures
{
    public class TemperatureSensorTimedOut : ITemperatureQueryReading
    {
        public static TemperatureSensorTimedOut Instance { get; } = new TemperatureSensorTimedOut();
        private TemperatureSensorTimedOut() { }
    }
}
