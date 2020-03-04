using Akka.Actor;
using Akka.TestKit.Xunit2;
using System;
using Xunit;
using BuildingMonitor.Actors;
using BuildingMonitor.Messages;

namespace BuildingMonitor.Tests
{
    public class TemperatureSensorSpecs : TestKit
    {
        [Fact]
        public void InitializeSensorMetadata()
        {
            var probe = CreateTestProbe();

            var sensor = Sys.ActorOf(TemperatureSensor.Props("a", "1"));

            sensor.Tell(new RequestMetadata(1), probe.Ref);

            var received = probe.ExpectMsg<ResponseMetadata>();

            Assert.Equal(1, received.RequestId);
            Assert.Equal("a", received.FloorId);
            Assert.Equal("1", received.SensorId);
        }

        [Fact]
        public void StartWithNoTemperature()
        {
            var probe = CreateTestProbe();

            var sensor = Sys.ActorOf(TemperatureSensor.Props("a", "1"));

            sensor.Tell(new RequestTemperature(1), probe.Ref);

            var received = probe.ExpectMsg<RespondTemperature>();

            Assert.Null(received.Temperature);
        }

        [Fact]
        public void ConfirmTemperatureUpdate()
        {
            var probe = CreateTestProbe();

            var sensor = Sys.ActorOf(TemperatureSensor.Props("a", "1"));

            sensor.Tell(new RequestUpdateTemperature(42, 100), probe.Ref);

            var received = probe.ExpectMsg<RespondTemperatureUpdated>(m => Assert.Equal(42, m.RequestId));
        }

        [Fact]
        public void RegisterSensor()
        {
            var probe = CreateTestProbe();

            var sensor = Sys.ActorOf(TemperatureSensor.Props("a", "1"));

            sensor.Tell(new RequestRegisterTemperatureSensor(42, "a", "1"), probe.Ref);

            var received = probe.ExpectMsg<RespondSensorRegistered>();

            Assert.Equal(42, received.RequestId);
            Assert.Equal(sensor, received.SensorReference);
        }

        [Fact]
        public void NoRegisterSensorWhenIncorrectFloor()
        {
            var probe = CreateTestProbe();
            var eventStreamProbe = CreateTestProbe();

            Sys.EventStream.Subscribe(eventStreamProbe, typeof(Akka.Event.UnhandledMessage));

            var sensor = Sys.ActorOf(TemperatureSensor.Props("a", "1"));

            sensor.Tell(new RequestRegisterTemperatureSensor(42, "b", "1"), probe.Ref);

            probe.ExpectNoMsg();

            var unhandled = eventStreamProbe.ExpectMsg<Akka.Event.UnhandledMessage>();

            Assert.IsType<RequestRegisterTemperatureSensor>(unhandled.Message);
        }
    }
}
