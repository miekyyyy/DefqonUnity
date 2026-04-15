using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace DefqonEngine.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimelineEvent
    {
        [JsonProperty] public float time;
        [JsonProperty] public float duration;
        [JsonProperty] public int trackIndex;
        [JsonProperty] public int targetId;
        [JsonProperty] public EventType type = EventType.Light; // Nieuw veld voor type
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventType
    {
        Light,
        Smoke
    }

}
