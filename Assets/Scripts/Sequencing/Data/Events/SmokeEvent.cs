using Newtonsoft.Json;

namespace DefqonEngine.Sequencing.Data.Events
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
