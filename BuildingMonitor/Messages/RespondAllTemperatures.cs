using BuildingMonitor.Messages.Temperatures;
using System.Collections.Immutable;

namespace BuildingMonitor.Messages
{
    public sealed class RespondAllTemperatures
    {
        public RespondAllTemperatures(long requestId, IImmutableDictionary<string, ITemperatureQueryReading> temperatures)
        {
            RequestId = requestId;
            Temperatures = temperatures;
        }

        public long RequestId { get; }
        public IImmutableDictionary<string, ITemperatureQueryReading> Temperatures { get; }
    }
}
