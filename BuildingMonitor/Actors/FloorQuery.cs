using Akka.Actor;
using BuildingMonitor.Messages;
using BuildingMonitor.Messages.Temperatures;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BuildingMonitor.Actors
{
    public class FloorQuery : UntypedActor
    {
        public static readonly long TemperatureRequestCorrelationId = 42;

        private ICancelable QueryTimeoutTimer { get; }

        private Dictionary<IActorRef, string> ActorToSensorId { get; }
        private long RequestId { get; }
        private IActorRef QueryRequester { get; }
        private TimeSpan Timeout { get; }
        private HashSet<IActorRef> StillAwaitingReplay { get; }
        private Dictionary<string, ITemperatureQueryReading> RepliesReceived { get; }

        public FloorQuery(Dictionary<IActorRef, string> actorToSensorId, long requestId, IActorRef queryRequester, TimeSpan timeout)
        {
            ActorToSensorId = actorToSensorId;
            RequestId = requestId;
            QueryRequester = queryRequester;
            Timeout = timeout;
            RepliesReceived = new Dictionary<string, ITemperatureQueryReading>();

            StillAwaitingReplay = new HashSet<IActorRef>(ActorToSensorId.Keys);

            QueryTimeoutTimer = Context.System.Scheduler.ScheduleTellOnceCancelable(timeout, Self, QueryTimeout.Instance, Self);
        }

        protected override void PreStart()
        {
            foreach (var temperatureSensor in ActorToSensorId.Keys)
            {
                Context.Watch(temperatureSensor);
                temperatureSensor.Tell(new RequestTemperature(TemperatureRequestCorrelationId));
            }
        }

        protected override void PostStop()
        {
            QueryTimeoutTimer.Cancel();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RespondTemperature m when m.RequestId == TemperatureRequestCorrelationId:
                    ITemperatureQueryReading reading = null;
                    if (m.Temperature.HasValue)
                    {
                        reading = new TemperatureAvailable(m.Temperature.Value);
                    }
                    else
                    {
                        reading = NoTemperatureReadingRecordedYet.Instance;
                    }
                    RecordSensorResponse(Sender, reading);
                    break;
                case QueryTimeout m:
                    foreach (var sensor in StillAwaitingReplay)
                    {
                        var sensorId = ActorToSensorId[sensor];
                        RepliesReceived.Add(sensorId, TemperatureSensorTimedOut.Instance);
                    }
                    QueryRequester.Tell(new RespondAllTemperatures(RequestId, RepliesReceived.ToImmutableDictionary()));
                    Context.Stop(Self);
                    break;
                case Terminated m:
                    RecordSensorResponse(m.ActorRef, TemperatureSensorNotAvailable.Instance);
                    break;
                default:
                    break;
            }
        }

        private void RecordSensorResponse(IActorRef sender, ITemperatureQueryReading reading)
        {
            Context.Unwatch(sender);

            var sensorId = ActorToSensorId[sender];

            StillAwaitingReplay.Remove(sender);
            RepliesReceived.Add(sensorId, reading);

            var allRepliesHaveBeenReceived = StillAwaitingReplay.Count == 0;

            if(allRepliesHaveBeenReceived)
            {
                QueryRequester.Tell(new RespondAllTemperatures(
                    RequestId,
                    RepliesReceived.ToImmutableDictionary()
                ));
                Context.Stop(Self);
            }
        }

        public static Props Props(Dictionary<IActorRef, string> actorToSensorId, long requestId, IActorRef queryRequester, TimeSpan timeout)
        {
            return Akka.Actor.Props.Create(() => new FloorQuery(actorToSensorId, requestId, queryRequester, timeout));
        }
    }
}
