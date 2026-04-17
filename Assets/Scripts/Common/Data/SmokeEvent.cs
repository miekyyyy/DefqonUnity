using DefqonEngine.Common;
using Newtonsoft.Json;
using EventType = DefqonEngine.Common.EventType;

namespace DefqonEngine.Lighting.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SmokeEvent : TimelineEvent
    {
        public SmokeEvent()
        {
            type = EventType.Smoke;
        }
    }

}
