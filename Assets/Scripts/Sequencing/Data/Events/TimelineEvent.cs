using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DefqonEngine.Sequencing.Data.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimelineEvent
    {
        [JsonProperty] public float time;
        [JsonProperty] public float duration;
        [JsonProperty] public int trackIndex;
        [JsonProperty] public int targetId;
        [JsonProperty] public EventType type = EventType.Light;
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventType
    {
        Light,
        Smoke
    }

}
