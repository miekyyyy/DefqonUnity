using DefqonEngine.Lighting.Data;
using System;
using TMPro;
using UnityEngine;

namespace DefqonEngine.Lighting.Runtime
{
    public class LightTimelinePlayer : MonoBehaviour
    {
        public AudioSource audioSource;
        public LightTimeline timeline;
        [SerializeField] TextMeshProUGUI timeDisplay;

        void Update()
        {
            if (audioSource == null || timeline == null) return;

            float t = audioSource.time;

            TimeSpan time = TimeSpan.FromSeconds(t);
            timeDisplay.text = time.ToString("mm':'ss':'ff");

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
