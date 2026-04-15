using DefqonEngine.Common;
using Newtonsoft.Json;

namespace DefqonEngine
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EventPreset
    {
        [JsonProperty] public string Name;
        [JsonProperty] public TimelineEvent timelineEvent;

        public EventPreset(string name, TimelineEvent timelineEvent)
        {
            Name = name;
            this.timelineEvent = timelineEvent;
        }
    }
}
