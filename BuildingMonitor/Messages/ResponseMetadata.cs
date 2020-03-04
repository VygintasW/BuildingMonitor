namespace BuildingMonitor.Messages
{
    public sealed class ResponseMetadata
    {
        public ResponseMetadata(long requestId, string floorId, string sensorId)
        {
            RequestId = requestId;
            FloorId = floorId;
            SensorId = sensorId;
        }

        public long RequestId { get; }
        public string FloorId { get; }
        public string SensorId { get; }
    }
}
