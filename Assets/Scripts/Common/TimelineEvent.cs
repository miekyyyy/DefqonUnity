using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Converters;
using UnityEngine;

namespace DefqonEngine
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
        Laser,
        Smoke,
        Servo
    }

}
