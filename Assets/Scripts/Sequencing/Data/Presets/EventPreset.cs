using DefqonEngine.Sequencing.Data.Events;
using Newtonsoft.Json;

namespace DefqonEngine.Sequencing.Data.Presets
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
