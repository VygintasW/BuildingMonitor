using Akka.Actor;
using BuildingMonitor.Messages;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace BuildingMonitor.Actors
{
    public class FloorsManager : UntypedActor
    {
        private Dictionary<string, IActorRef> Floors { get; }
        public FloorsManager()
        {
            Floors = new Dictionary<string, IActorRef>();
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestRegisterTemperatureSensor m:
                    if (!Floors.TryGetValue(m.FloorId, out var floorActorRef))
                    {
                        floorActorRef = Context.ActorOf(Floor.Props(m.FloorId), $"floor-{m.FloorId}");
                        Floors.Add(m.FloorId, floorActorRef);
                        Context.Watch(floorActorRef);
                    }
                    floorActorRef.Forward(m);
                    break;
                case RequestFloorIds m:
                    Sender.Tell(new RespondFloorIds(m.RequestId, ImmutableHashSet.CreateRange(Floors.Keys)));
                    break;
                case Terminated m:
                    var termintedFloorId = Floors.First(x => x.Value == m.ActorRef).Key;
                    Floors.Remove(termintedFloorId);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<FloorsManager>();
        }
    }
}
