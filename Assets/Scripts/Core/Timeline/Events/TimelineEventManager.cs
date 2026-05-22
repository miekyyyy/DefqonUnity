using DefqonEngine.Core.Timeline.History;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Sequencing.Data.Presets;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Control;
using DefqonEngine.UI.Timeline.Events;
using DefqonEngine.UI.Timeline.Tracks;
using System;
using System.Collections.Generic;
using UnityEngine;
using EventType = DefqonEngine.Sequencing.Data.Events.EventType;

namespace DefqonEngine.Core.Timeline.Events
{
    public class TimelineEventManager : MonoBehaviour
    {
        public static TimelineEventManager Instance { get; private set; }

        public List<TimelineEvent> events = new();
        public List<TimelineEvent> selectedEvents = new();

        bool hasTrackChanged;

        // Event callbacks
        public event Action<TimelineEvent> OnEventAdded;
        public event Action<TimelineEvent> OnEventRemoved;
        public event Action<TimelineEvent> OnEventSelected;
        public event Action OnEventDeselected;

        // Views
        private Dictionary<TimelineEvent, TimelineEventView> views = new(); // Map van data naar view
        public TimelineEventView viewPrefab;   // Prefab voor event views

        private void Awake() => Instance = this;

        private void Start()
        {
            TimelineInputController.Instance.OnMoveUp += MoveEventUp;
            TimelineInputController.Instance.OnMoveDown += MoveEventDown;
            TimelineInputController.Instance.OnControlUp += FinishTrackChange;
        }

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

        public void CreateEvent(TimelineEvent timelineEvent, float time)
        {
            if (timelineEvent == null)
            {
                Debug.LogError("Invalid timeline event for creating event.");
                return;
            }
            TimelineEvent ev = CloneEvent(timelineEvent);
            ev.time = time;
            AddEvent(ev);
            ApplyTrim(ev, ev.time);
            SelectEvent(ev);
        }

        public void AddEvent(TimelineEvent timelineEvent, bool saveState = true)
        {
            if (saveState)
            {
                TimelineHistory.Instance.SaveState("Adding Event");
            }

            //Data
            events.Add(timelineEvent);

            //Visual
            TimelineTrack track = TimelineTrackManager.Instance.FindTrackByIndex(timelineEvent.trackIndex);
            if (track == null)
            {
                Debug.LogWarning($"Track {timelineEvent.trackIndex} niet gevonden voor event.");
                return;
            }

            TimelineEventView view = Instantiate(viewPrefab, TimelineView.Instance.eventsPanel);
            view.Initialize(timelineEvent, track);
            views[timelineEvent] = view;

            OnEventAdded?.Invoke(timelineEvent);
        }

        public List<TimelineEvent> GetEventsCopy()
        {
            List<TimelineEvent> copy = new();
            foreach (var ev in events)
                copy.Add(CloneEvent(ev));
            return copy;
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

        public void ClearEvents()
        {
            foreach (var ev in new List<TimelineEvent>(events))
                RemoveEvent(ev, false);
        }

        public void RemoveEvent(TimelineEvent timelineEvent, bool saveState = true)
        {
            if (saveState)
            {
                TimelineHistory.Instance.SaveState("Removing Event");
            }

            // Data
            if (!events.Remove(timelineEvent))
                return;

            // Visual
            if (views.TryGetValue(timelineEvent, out var view))
            {
                Destroy(view.gameObject);
                views.Remove(timelineEvent);
            }

            if (selectedEvents.Contains(timelineEvent))
                DeselectEvent(timelineEvent);

            OnEventRemoved?.Invoke(timelineEvent);
        }

        public void RemoveEventSelected()
        {
            if (selectedEvents != null)
                foreach (var ev in selectedEvents)
                    RemoveEvent(ev);
        }

        public void SetEvents(List<TimelineEvent> incomingEvents)
        {
            // Verwijder eerst alle bestaande events en views
            foreach (var ev in new List<TimelineEvent>(events))
                RemoveEvent(ev, false);

            // Voeg nieuwe events toe met visuals
            foreach (var ev in incomingEvents)
                AddEvent(ev, false);
            DeselectEvent();
        }

        #endregion

        #region Selection
        public void SelectEvent(TimelineEvent ev)
        {
            FinishTrackChange();
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (selectedEvents.Contains(ev))
                {
                    DeselectEvent(ev);
                    return;
                }
                else
                {
                    selectedEvents.Add(ev);
                }
            }
            else
            {
                DeselectEvent();
                selectedEvents = new List<TimelineEvent> { ev };
            }

            // Highlight view als die bestaat
            if (views.TryGetValue(ev, out var view))
                view.Select();

            OnEventSelected?.Invoke(ev);
        }

        public void DeselectEvent(TimelineEvent eventToRemove = null)
        {
            FinishTrackChange();
            if (selectedEvents == null)
                return;
            if (eventToRemove != null)
            {
                if (views.TryGetValue(eventToRemove, out var view))
                    view.Deselect();
                if (selectedEvents.Contains(eventToRemove))
                    selectedEvents.Remove(eventToRemove);
            }
            else
            {
                foreach (var ev in selectedEvents)
                {
                    if (views.TryGetValue(ev, out var view))
                        view.Deselect();
                }
                selectedEvents.Clear();
            }

            OnEventDeselected?.Invoke();
        }
        #endregion

        #region Utility
        public TimelineEventView GetView(TimelineEvent ev)
        {
            views.TryGetValue(ev, out var view);
            return view;
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
                RemoveEvent(r, false);

            // daarna nieuwe delen toevoegen
            foreach (var a in toAdd)
                AddEvent(a, false);

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

        public void MoveEventUp()
        {
            if (selectedEvents == null)
                return;
            foreach (var e in selectedEvents)
            {
                int newTrackIndex = e.trackIndex - 1;
                if (views.TryGetValue(e, out var view))
                {
                    view.ChangeTrack(newTrackIndex, true);
                    hasTrackChanged = true;
                }
            }
        }

        public void MoveEventDown()
        {
            if (selectedEvents == null)
                return;
            foreach (var e in selectedEvents)
            {
                int newTrackIndex = e.trackIndex + 1;
                if (views.TryGetValue(e, out var view))
                {
                    view.ChangeTrack(newTrackIndex, true);
                    hasTrackChanged = true;
                }
            }
        }

        public void FinishTrackChange()
        {
            Debug.Log("Finish track change");
            if (selectedEvents == null)
                return;
            if(!hasTrackChanged)
                return;

            foreach (var e in selectedEvents)
            {
                if (views.TryGetValue(e, out var view))
                {
                    view.ChangeTrack(e.trackIndex, true, true);
                    hasTrackChanged = false;
                }
            }
        }
        #endregion
    }
}