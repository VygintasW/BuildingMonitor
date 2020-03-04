namespace BuildingMonitor.Messages.Temperatures
{
    public sealed class NoTemperatureReadingRecordedYet : ITemperatureQueryReading
    {
        public static NoTemperatureReadingRecordedYet Instance { get; } = new NoTemperatureReadingRecordedYet();
        private NoTemperatureReadingRecordedYet() { }
    }
}
