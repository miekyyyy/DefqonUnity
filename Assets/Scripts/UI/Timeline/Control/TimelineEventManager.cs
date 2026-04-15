using DefqonEngine.Common;
using DefqonEngine.Lighting.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using EventType = DefqonEngine.Common.EventType;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineEventManager : MonoBehaviour
    {
        public static TimelineEventManager Instance { get; private set; }

        public List<TimelineEvent> events = new();
        public TimelineEvent selectedEvent;

        // Event callbacks
        public event Action<TimelineEvent> OnEventAdded;
        public event Action<TimelineEvent> OnEventRemoved;
        public event Action<TimelineEvent> OnEventSelected;
        public event Action OnEventDeselected;

        // Views
        private Dictionary<TimelineEvent, TimelineEventView> views = new(); // Map van data naar view
        public Transform eventsLayer;           // Parent voor alle event visuals
        public TimelineEventView viewPrefab;   // Prefab voor event views

        private void Awake() => Instance = this;


        #region Event CRUD
        public void CreateEvent<T>(int trackIndex, int targetId, float time, float duration = 1f) where T : TimelineEvent, new()
        {
            T ev = new T
            {
                trackIndex = trackIndex,
                targetId = targetId,
                time = time,
                duration = duration
            };

            AddEvent(ev);
            SelectEvent(ev);
        }

        public void AddEvent(TimelineEvent timelineEvent)
        {
            //Data
            events.Add(timelineEvent);

            //Visual
            TimelineTrack track = TimelineTrackManager.Instance.FindTrackByIndex(timelineEvent.trackIndex);
            if (track == null)
            {
                Debug.LogWarning($"Track {timelineEvent.trackIndex} niet gevonden voor event.");
                return;
            }

            TimelineEventView view = Instantiate(viewPrefab, eventsLayer);
            view.Initialize(timelineEvent, track);
            views[timelineEvent] = view;

            OnEventAdded?.Invoke(timelineEvent);
        }

        public void ReplaceEvent(String eventType)
        {
            if (!Enum.TryParse(eventType, out EventType parsedType))
            {
                Debug.LogError($"Invalid event type: {eventType}");
                return;
            }

            if (selectedEvent == null)
            {
                Debug.LogWarning("No event selected to replace.");
                return;
            }

            EventType type = parsedType;
            TimelineEvent newEvent = type switch
            {
                EventType.Light => new LightEvent
                {
                    trackIndex = selectedEvent.trackIndex,
                    targetId = selectedEvent.targetId,
                    time = selectedEvent.time,
                    duration = selectedEvent.duration
                },
                EventType.Smoke => new SmokeEvent
                {
                    trackIndex = selectedEvent.trackIndex,
                    targetId = selectedEvent.targetId,
                    time = selectedEvent.time,
                    duration = selectedEvent.duration
                },
                _ => throw new NotImplementedException()
            };
            RemoveEvent(selectedEvent);
            AddEvent(newEvent);
        }

        public void RemoveEvent(TimelineEvent timelineEvent)
        {
            // Data
            if (!events.Remove(timelineEvent))
                return;

            // Visual
            if (views.TryGetValue(timelineEvent, out var view))
            {
                Destroy(view.gameObject);
                views.Remove(timelineEvent);
            }

            if(selectedEvent == timelineEvent)
                DeselectEvent();

            OnEventRemoved?.Invoke(timelineEvent);
        }

        public void RemoveEventSelected()
        {
            if (selectedEvent != null)
                RemoveEvent(selectedEvent);
        }

        public void SetEvents(List<TimelineEvent> incomingEvents)
        {
            // Verwijder eerst alle bestaande events en views
            foreach (var ev in new List<TimelineEvent>(events))
                RemoveEvent(ev);

            // Voeg nieuwe events toe met visuals
            foreach (var ev in incomingEvents)
                AddEvent(ev);
            DeselectEvent();
        }

        #endregion

        #region Selection
        public void SelectEvent(TimelineEvent ev)
        {
            DeselectEvent();
            selectedEvent = ev;

            // Highlight view als die bestaat
            if (views.TryGetValue(ev, out var view))
                view.Select();

            OnEventSelected?.Invoke(ev);
        }

        public void DeselectEvent()
        {
            if (selectedEvent == null)
                return;

            if (views.TryGetValue(selectedEvent, out var view))
                view.Deselect();

            selectedEvent = null;
            OnEventDeselected?.Invoke();
        }
        #endregion

        #region Utility
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
        #endregion
    }
}