using Akka.Actor;
using BuildingMonitor.Messages;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BuildingMonitor.Actors
{
    public class Floor : UntypedActor
    {
        public Floor(string floorId)
        {
            FloorId = floorId;
            Sensors = new Dictionary<string, IActorRef>();
        }

        private string FloorId { get; }
        private Dictionary<string, IActorRef> Sensors { get; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestRegisterTemperatureSensor m when m.FloorId == FloorId:
                    if (!Sensors.TryGetValue(m.SensorId, out var existingSensorActorRef))
                    {
                        existingSensorActorRef = Context.ActorOf(TemperatureSensor.Props(FloorId, m.SensorId), $"temperature-sensor{m.SensorId}");
                        Sensors.Add(m.SensorId, existingSensorActorRef);
                        Context.Watch(existingSensorActorRef);
                    }
                    existingSensorActorRef.Forward(m);
                    break;
                case RequestTemperatureSensorIds m:
                    Sender.Tell(new RespondTemperatureSensorIds(m.RequestId, ImmutableHashSet.CreateRange<string>(Sensors.Keys)));
                    break;
                case RequestAllTemperatures m:
                    var actorRefToSensorIdMap = new Dictionary<IActorRef, string>();
                    foreach (var item in Sensors)
                    {
                        actorRefToSensorIdMap.Add(item.Value, item.Key);
                    }
                    Context.ActorOf(FloorQuery.Props(actorRefToSensorIdMap, m.RequestId, Sender, TimeSpan.FromSeconds(3)));
                    break;
                case Terminated m:
                    var termintedTemperatureSensorId = Sensors.First(x => x.Value == m.ActorRef).Key;
                    Sensors.Remove(termintedTemperatureSensorId);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        public static Props Props(string floorId)
        {
            return Akka.Actor.Props.Create(() => new Floor(floorId));

        }
    }
}
