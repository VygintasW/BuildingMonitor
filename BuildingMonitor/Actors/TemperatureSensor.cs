using Akka.Actor;
using BuildingMonitor.Messages;

namespace BuildingMonitor.Actors
{
    public class TemperatureSensor : UntypedActor
    {
        public string FloorId { get; }
        public string SensorId { get; }

        private double? LastRecordedTemperature { get; set; }

        public TemperatureSensor(string floorId, string sensorId)
        {
            FloorId = floorId;
            SensorId = sensorId;
        }
        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case RequestMetadata m:
                    Sender.Tell(new ResponseMetadata(m.RequestId, FloorId, SensorId));
                    break;
                case RequestTemperature m:
                    Sender.Tell(new RespondTemperature(m.RequestId, LastRecordedTemperature));
                    break;
                case RequestUpdateTemperature m:
                    LastRecordedTemperature = m.Temperature;
                    Sender.Tell(new RespondTemperatureUpdated(m.RequestId));
                    break;
                case RequestRegisterTemperatureSensor m when m.FloorId == FloorId && m.SensorId == SensorId:
                    Sender.Tell(new RespondSensorRegistered(m.RequestId, Context.Self));
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }

        public static Props Props(string floorId, string sensorId)
        {
            return Akka.Actor.Props.Create(() => new TemperatureSensor(floorId, sensorId));
        }
    }
}
