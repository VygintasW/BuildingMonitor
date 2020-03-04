namespace BuildingMonitor.Messages
{
    public sealed class RequestTemperatureSensorIds
    {
        public RequestTemperatureSensorIds(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
