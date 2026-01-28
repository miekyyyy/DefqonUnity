using System;
using System.Collections.Generic;
using UnityEngine;
using DefqonEngine.Lighting.Data;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineEventManager : MonoBehaviour
    {
        public static TimelineEventManager Instance { get; private set; }

        public List<LightEvent> events = new();

        public event Action<LightEvent> OnEventAdded;

        void Awake() => Instance = this;

        public void CreateEvent(int trackIndex, float time, float duration = 1f)
        {
            LightEvent ev = new LightEvent
            {
                trackIndex = trackIndex,
                time = time,
                duration = duration,
                color = Color.white
            };

            events.Add(ev);

            OnEventAdded?.Invoke(ev);
        }
    }
}
