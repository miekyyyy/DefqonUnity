using Newtonsoft.Json;

namespace DefqonEngine.Common.Data
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
