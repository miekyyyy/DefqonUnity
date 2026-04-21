using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Sequencing.Data.Presets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DefqonEngine.IO.Project
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DefqonProject
    {
        [JsonProperty] public string projectName;
        [JsonProperty] public List<TimelineEvent> events = new List<TimelineEvent>();
        [JsonProperty] public List<EventPreset> presets = new List<EventPreset>();
        [JsonProperty] public string audioFilePath;
        [JsonProperty] public int trackCount;
        public DefqonProject(string projectName, string audioFilePath)
        {
            this.projectName = projectName;
            this.audioFilePath = audioFilePath;
        }
    }
}
