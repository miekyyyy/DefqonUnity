using System;
using System.Collections.Generic;
using UnityEngine;
using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineEventManager : MonoBehaviour
    {
        public static TimelineEventManager Instance { get; private set; }

        public List<TimelineEvent> events = new();

        public event Action<TimelineEvent> OnEventAdded;
        public event Action OnEventRemoved;

        void Awake() => Instance = this;


        public void CreateEvent<T>(int trackIndex, int targetId, float time, float duration = 1f) where T : TimelineEvent, new()
        {
            T ev = new T
            {
                trackIndex = trackIndex,
                time = time,
                targetId = targetId,
                duration = duration
            };

            events.Add(ev);
            OnEventAdded?.Invoke(ev);
        }


        public void RemoveEvent()
        {
            if (TimelineEventViewManager.Instance.selectedView == null)
                return;

            TimelineEvent selectedTimelineEvent = TimelineEventViewManager.Instance.selectedView.lightEvent;

            events.Remove(selectedTimelineEvent);
            OnEventRemoved?.Invoke();
        }

        public void RemoveEvent(TimelineEvent timelineEvent)
        {
            events.Remove(timelineEvent);
            OnEventRemoved?.Invoke();
        }

        public TimelineEvent GetPreviousEvent(TimelineEvent ev)
        {
            TimelineEvent prev = null;

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

        public TimelineEvent GetNextEvent(TimelineEvent ev)
        {
            TimelineEvent next = null;

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

        public void SetEvents(List<TimelineEvent> events)
        {
            foreach (var timelineEvent in events)
            {
                RemoveEvent(timelineEvent);
            }
            this.events = events;
        }
    }
}
