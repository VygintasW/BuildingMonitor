using Akka.Actor;

namespace BuildingMonitor.Messages
{
    public sealed class RespondSensorRegistered
    {
        public RespondSensorRegistered(long requestId, IActorRef sensorReference)
        {
            RequestId = requestId;
            SensorReference = sensorReference;
        }

        public long RequestId { get; }
        public IActorRef SensorReference { get; }
    }
}
