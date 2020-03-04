namespace BuildingMonitor.Messages
{
    public class RequestFloorIds
    {
        public RequestFloorIds(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
