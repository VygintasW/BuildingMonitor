using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingMonitor.Actors;
using BuildingMonitor.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BuildingMonitor.Tests
{
    public class FloorsManagerSpecs : TestKit
    {
        [Fact]
        public void ReturnNoFloorIdsWhenNewlyCreated()
        {
            var probe = CreateTestProbe();
            var manager = Sys.ActorOf(FloorsManager.Props());

            manager.Tell(new RequestFloorIds(1), probe.Ref);
            var received = probe.ExpectMsg<RespondFloorIds>();

            Assert.Equal(1, received.RequestId);
            Assert.Empty(received.Ids);
        }

        [Fact]
        public void RegisterNewFloorWhenDoesNotAlreadyExists()
        {
            var probe = CreateTestProbe();
            var manager = Sys.ActorOf(FloorsManager.Props());

            manager.Tell(new RequestRegisterTemperatureSensor(1, "a", "42"), probe.Ref);
            probe.ExpectMsg<RespondSensorRegistered>(x => x.RequestId == 1);

            manager.Tell(new RequestFloorIds(2), probe.Ref);
            var received = probe.ExpectMsg<RespondFloorIds>();

            Assert.Equal(2, received.RequestId);
            Assert.Single(received.Ids);
            Assert.Contains("a", received.Ids);
        }

        [Fact]
        public void ReuseExistingFloorWhenAlreadyExists()
        {
            var probe = CreateTestProbe();
            var manager = Sys.ActorOf(FloorsManager.Props());

            manager.Tell(new RequestRegisterTemperatureSensor(1, "a", "42"), probe.Ref);
            probe.ExpectMsg<RespondSensorRegistered>(x => x.RequestId == 1);

            manager.Tell(new RequestRegisterTemperatureSensor(2, "a", "90"), probe.Ref);
            probe.ExpectMsg<RespondSensorRegistered>(x => x.RequestId == 2);

            manager.Tell(new RequestFloorIds(3), probe.Ref);
            var received = probe.ExpectMsg<RespondFloorIds>();

            Assert.Equal(3, received.RequestId);
            Assert.Single(received.Ids);
            Assert.Contains("a", received.Ids);
        }

        [Fact]
        public async Task ReturnFloorIdsOnlyFromActiveActors()
        {
            var probe = CreateTestProbe();
            var floorsManager = Sys.ActorOf(FloorsManager.Props(), "FloorsManager");

            floorsManager.Tell(new RequestRegisterTemperatureSensor(1, "a", "42"), probe.Ref);
            probe.ExpectMsg<RespondSensorRegistered>(x => x.RequestId == 1);

            floorsManager.Tell(new RequestRegisterTemperatureSensor(2, "b", "90"), probe.Ref);
            probe.ExpectMsg<RespondSensorRegistered>(x => x.RequestId == 2);

            var firstFloor = await Sys.ActorSelection("akka://test/user/FloorsManager/floor-a")
                                      .ResolveOne(TimeSpan.FromSeconds(3));
            probe.Watch(firstFloor);

            firstFloor.Tell(PoisonPill.Instance);

            probe.ExpectTerminated(firstFloor);

            floorsManager.Tell(new RequestFloorIds(3), probe.Ref);
            var received = probe.ExpectMsg<RespondFloorIds>();

            Assert.Equal(3, received.RequestId);
            Assert.Single(received.Ids);
            Assert.Contains("b", received.Ids.Single());
        }
    }
}
