using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Converters;
using UnityEngine;

namespace DefqonEngine.Lighting.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LightEvent : TimelineEvent
    {
        [JsonProperty] public LightEffectType lightEffectType;
        [JsonProperty] public Color color = Color.white;
        [JsonProperty] public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);

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

}
