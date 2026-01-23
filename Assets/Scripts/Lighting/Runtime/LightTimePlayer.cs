using UnityEngine;
using DefqonEngine.Lighting.Data;

namespace DefqonEngine.Lighting.Runtime
{
    public class LightTimelinePlayer : MonoBehaviour
    {
        public AudioSource audioSource;
        public LightTimeline timeline;

        void Update()
        {
            if (audioSource == null || timeline == null) return;

            float t = audioSource.time;

            foreach (var e in timeline.events)
            {
                if (t >= e.time && t <= e.time + e.duration)
                {
                    LightManager.Instance.ApplyEvent(e);
                }
            }
        }
    }
}
