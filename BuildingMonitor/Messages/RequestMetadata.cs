namespace BuildingMonitor.Messages
{
    public sealed class RequestMetadata
    {
        public RequestMetadata(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
