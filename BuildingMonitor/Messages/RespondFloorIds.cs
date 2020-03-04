using System.Collections.Immutable;

namespace BuildingMonitor.Messages
{
    public class RespondFloorIds
    {
        public RespondFloorIds(long requestId, ImmutableHashSet<string> ids)
        {
            RequestId = requestId;
            Ids = ids;
        }

        public long RequestId { get; }
        public ImmutableHashSet<string> Ids { get; }
    }
}
