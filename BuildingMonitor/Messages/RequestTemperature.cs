namespace BuildingMonitor.Messages
{
    public sealed class RequestTemperature
    {
        public RequestTemperature(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
