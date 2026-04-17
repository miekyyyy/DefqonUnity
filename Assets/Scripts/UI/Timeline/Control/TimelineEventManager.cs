using DefqonEngine.Common.Data;
using DefqonEngine.UI.Timeline.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using EventType = DefqonEngine.Common.Data.EventType;

namespace DefqonEngine.UI.Timeline.Control
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
            ApplyTrim(ev, ev.time);
            SelectEvent(ev);
        }

        public void CreateEvent(EventPreset preset, int trackIndex, float time, float duration = 1f)
        {
            if (preset == null || preset.timelineEvent == null)
            {
                Debug.LogError("Invalid preset for creating event.");
                return;
            }
            TimelineEvent ev = CloneEvent(preset.timelineEvent);
            ev.trackIndex = trackIndex;
            ev.time = time;
            ev.duration = duration;
            AddEvent(ev);
            ApplyTrim(ev, ev.time);
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

        public static T CloneEvent<T>(T source) where T : TimelineEvent, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is LightEvent lightEvent)
            {
                if (source.type != EventType.Light)
                {
                    throw new InvalidOperationException($"Timeline event runtime type '{nameof(LightEvent)}' does not match declared type '{source.type}'.");
                }

                return (T)(TimelineEvent)new LightEvent
                {
                    trackIndex = source.trackIndex,
                    targetId = source.targetId,
                    time = source.time,
                    duration = source.duration,
                    lightEffectType = lightEvent.lightEffectType,
                    color = lightEvent.color,
                    fadeCurve = lightEvent.fadeCurve,
                    inverted = lightEvent.inverted,
                    type = source.type
                };
            }

            if (source is SmokeEvent smokeEvent)
            {
                if (source.type != EventType.Smoke)
                {
                    throw new InvalidOperationException($"Timeline event runtime type '{nameof(SmokeEvent)}' does not match declared type '{source.type}'.");
                }

                return (T)(TimelineEvent)new SmokeEvent
                {
                    trackIndex = source.trackIndex,
                    targetId = source.targetId,
                    time = source.time,
                    duration = source.duration,
                    type = source.type
                };
            }

            throw new NotImplementedException($"Cloning is not implemented for timeline event runtime type '{source.GetType().Name}'.");
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

            if (selectedEvent == timelineEvent)
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

        public void ApplyTrim(TimelineEvent ev, float newStart)
        {
            float start = Mathf.Max(0f, newStart);
            float end = start + ev.duration;

            List<TimelineEvent> modified = new();
            List<TimelineEvent> toRemove = new();
            List<TimelineEvent> toAdd = new();

            foreach (var other in events)
            {
                if (other == ev || other.trackIndex != ev.trackIndex)
                    continue;

                float oStart = other.time;
                float oEnd = other.time + other.duration;

                // nieuw event split oud event
                if (oStart < start && oEnd > end)
                {
                    float leftDur = start - oStart;
                    float rightDur = oEnd - end;

                    // rechter deel
                    if (rightDur >= 0.05f)
                    {
                        TimelineEvent rightPart = CloneEvent(other);
                        rightPart.time = end;
                        rightPart.duration = rightDur;
                        toAdd.Add(rightPart);
                    }

                    // linker deel
                    if (leftDur >= 0.05f)
                    {
                        other.duration = leftDur;
                        modified.Add(other);
                    }
                    else
                    {
                        toRemove.Add(other);
                    }

                    continue;
                }

                // nieuw event overlapt oud event volledig 
                if (oStart >= start && oEnd <= end)
                {
                    toRemove.Add(other);
                    continue;
                }

                // nieuw event overlapt oud event links
                if (oStart < start && oEnd > start)
                {
                    float newDur = start - oStart;

                    if (newDur >= 0.05f)
                    {
                        other.duration = newDur;
                        modified.Add(other);
                    }
                    else
                    {
                        toRemove.Add(other);
                    }
                }

                // nieuw event overlapt oud event rechts
                if (oStart < end && oEnd > end)
                {
                    float newDur = oEnd - end;

                    if (newDur >= 0.05f)
                    {
                        other.time = end;
                        other.duration = newDur;
                        modified.Add(other);
                    }
                    else
                    {
                        toRemove.Add(other);
                    }
                }
            }

            // eerst verwijderen
            foreach (var r in toRemove)
                RemoveEvent(r);

            // daarna nieuwe delen toevoegen
            foreach (var a in toAdd)
                AddEvent(a);

            // daarna bestaande events updaten
            foreach (var m in modified)
            {
                if (views.TryGetValue(m, out var view))
                    view.UpdateVisual();
            }

            ev.time = start;
            ev.duration = ClampDuration(ev.duration);
        }

        float ClampDuration(float duration)
        {
            return Mathf.Max(0.05f, duration);
        }
        #endregion
    }
}