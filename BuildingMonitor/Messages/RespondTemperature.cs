namespace BuildingMonitor.Messages
{
    public sealed class RespondTemperature
    {
        public RespondTemperature(long requestId, double? temperature)
        {
            RequestId = requestId;
            Temperature = temperature;
        }

        public long RequestId { get; }
        public double? Temperature { get; }
    }
}
