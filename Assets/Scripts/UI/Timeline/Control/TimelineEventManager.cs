using DefqonEngine.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineEventManager : MonoBehaviour
    {
        public static TimelineEventManager Instance { get; private set; }

        public List<TimelineEvent> events = new();

        public event Action<TimelineEvent> OnEventAdded;
        public event Action OnEventRemoved;
        public event Action<TimelineEvent> OnEventRemovedSpecified;
        public event Action<TimelineEvent> OnEventSelected;
        public event Action OnEventDeselected;

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

            TimelineEvent selectedTimelineEvent = TimelineEventViewManager.Instance.selectedView.timelineEvent;

            events.Remove(selectedTimelineEvent);
            OnEventRemoved?.Invoke();
        }

        public void RemoveEvent(TimelineEvent timelineEvent)
        {
            events.Remove(timelineEvent);
            OnEventRemovedSpecified?.Invoke(timelineEvent);
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

        public void SetEvents(List<TimelineEvent> incomingEvents)
        {
            foreach (var timelineEvent in new List<TimelineEvent>(events))
            {
                RemoveEvent(timelineEvent);
            }

            events.Clear();
            events = incomingEvents;
        }

        public void SelectEvent(TimelineEvent lightEvent)
        {
            DeselectEvent();
            OnEventSelected.Invoke(lightEvent);
        }
        public void DeselectEvent()
        {
            OnEventDeselected.Invoke();
        }
    }
}
