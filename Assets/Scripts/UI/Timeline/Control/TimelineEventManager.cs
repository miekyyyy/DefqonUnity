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
        public event Action OnEventRemoved;

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

        public void RemoveEvent()
        {
            if (TimelineEventViewManager.Instance.selectedView == null)
                return;

            LightEvent lightEvent = TimelineEventViewManager.Instance.selectedView.lightEvent;

            events.Remove(lightEvent);
            OnEventRemoved?.Invoke();
        }

        public LightEvent GetPreviousEvent(LightEvent ev)
        {
            LightEvent prev = null;

            foreach (var e in events)
            {
                if (e == ev || e.trackIndex != ev.trackIndex)
                    continue;

                if (e.time + e.duration <= ev.time)
                {
                    if (prev == null || e.time > prev.time)
                        prev = e;
                }
            }

            return prev;
        }

        public LightEvent GetNextEvent(LightEvent ev)
        {
            LightEvent next = null;

            foreach (var e in events)
            {
                if (e == ev || e.trackIndex != ev.trackIndex)
                    continue;

                if (e.time >= ev.time + ev.duration)
                {
                    if (next == null || e.time < next.time)
                        next = e;
                }
            }

            return next;
        }

    }
}
