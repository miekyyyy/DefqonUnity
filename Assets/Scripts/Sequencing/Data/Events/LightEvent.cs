using DefqonEngine.IO.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace DefqonEngine.Sequencing.Data.Events
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LightEvent : TimelineEvent
    {
        [JsonProperty] public LightEffectType lightEffectType;
        [JsonProperty] public SerializableColor color = Color.white;
        [JsonProperty] public CurveType fadeCurve = CurveType.Linear;
        [JsonProperty] public bool inverted = false;

        public LightEvent()
        {
            type = EventType.Light;
        }
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LightEffectType
    {
        Static,
        Fade,
        Chase,
        Converge,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CurveType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
    }

}
