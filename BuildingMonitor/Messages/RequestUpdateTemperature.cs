namespace BuildingMonitor.Messages
{
    public sealed class RequestUpdateTemperature
    {
        public RequestUpdateTemperature(long requestId, double temperature)
        {
            RequestId = requestId;
            Temperature = temperature;
        }

        public long RequestId { get; }
        public double Temperature { get; }
    }
}
