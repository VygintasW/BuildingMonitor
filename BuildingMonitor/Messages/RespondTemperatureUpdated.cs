namespace BuildingMonitor.Messages
{
    public sealed class RespondTemperatureUpdated
    {
        public RespondTemperatureUpdated(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
