using System.Collections.Generic;

namespace DefqonEngine.Lighting.Data
{
    [System.Serializable]
    public class LightTimeline
    {
        public List<LightEvent> events = new();
    }
}
