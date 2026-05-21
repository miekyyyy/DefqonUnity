using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Audio;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Stage.Fixtures.Management;
using System.Collections.Generic;
using UnityEngine;
using EventType = DefqonEngine.Sequencing.Data.Events.EventType;

namespace DefqonEngine.Sequencing.Playback
{
    public class EventTimelineRuntime : MonoBehaviour
    {
        private struct ActiveTimelineEvent
        {
            public int priority;
            public TimelineEvent evt;
        }

        private float _currentTime;

        void LateUpdate()
        {
            _currentTime = AudioPlaybackController.Instance.GetCurrentTime();

            var perTargetEvents = CollectActiveEvents();

            ApplyLightEvents(perTargetEvents);
            ApplySmokeEffect(perTargetEvents);
        }

        private Dictionary<(EventType type, int id), List<ActiveTimelineEvent>> CollectActiveEvents()
        {
            var result = new Dictionary<(EventType type, int id), List<ActiveTimelineEvent>>();

            foreach (var timelineEvent in TimelineEventManager.Instance.events)
            {
                if (!IsActive(timelineEvent))
                    continue;

                switch (timelineEvent.type)
                {
                    case EventType.Light:
                        AddLightEvent((LightEvent)timelineEvent, result);
                        break;
                    case EventType.Smoke:
                        AddSmokeEvent((SmokeEvent)timelineEvent, result);
                        break;
                }
            }

            return result;
        }

        private void AddSmokeEvent(SmokeEvent timelineEvent,
            Dictionary<(EventType type, int id), List<ActiveTimelineEvent>> dict)
        {
            if (!SmokeManager.Instance.groups.TryGetValue(timelineEvent.targetId, out var group))
                return;

            foreach (var smokeId in group.smokeIds)
            {
                var key = (EventType.Smoke, smokeId);
                if (!dict.TryGetValue(key, out var list))
                {
                    list = new List<ActiveTimelineEvent>();
                    dict[key] = list;
                }
                list.Add(new ActiveTimelineEvent
                {
                    priority = group.priority,
                    evt = timelineEvent
                });
            }
        }

        private void AddLightEvent(
            LightEvent lightEvent,
            Dictionary<(EventType type, int id), List<ActiveTimelineEvent>> dict)
        {
            if (!LightManager.Instance.groups.TryGetValue(lightEvent.targetId, out var group))
                return;

            foreach (var lampId in group.lampIds)
            {
                var key = (EventType.Light, lampId);

                if (!dict.TryGetValue(key, out var list))
                {
                    list = new List<ActiveTimelineEvent>();
                    dict[key] = list;
                }

                list.Add(new ActiveTimelineEvent
                {
                    priority = group.priority,
                    evt = lightEvent
                });
            }
        }

        private void ApplySmokeEffect(
            Dictionary<(EventType type, int id), List<ActiveTimelineEvent>> perTargetEvents)
        {
            foreach (var smokeEntry in SmokeManager.Instance.smokeMachines)
            {
                var smokeId = smokeEntry.Key;
                var smokeMachine = smokeEntry.Value;

                bool active = false;

                if (perTargetEvents.TryGetValue((EventType.Smoke, smokeId), out var events))
                {
                    foreach (var e in events)
                    {
                        active |= IsActive(e.evt);
                    }
                }

                smokeMachine.Trigger(active);
            }
        }

        private void ApplyLightEvents(
            Dictionary<(EventType type, int id), List<ActiveTimelineEvent>> perTargetEvents)
        {
            foreach (var lampEntry in LightManager.Instance.lamps)
            {
                var lampId = lampEntry.Key;
                var lamp = lampEntry.Value;

                Color color = Color.black;

                if (perTargetEvents.TryGetValue((EventType.Light, lampId), out var events))
                {
                    events.Sort((a, b) => a.priority.CompareTo(b.priority));

                    foreach (var active in events)
                    {
                        if (LightEventEvaluator.Evaluate(
                            (LightEvent)active.evt,
                            _currentTime,
                            color,
                            out Color result))
                        {
                            color = result;
                        }
                    }
                }

                lamp.SetColor(color);
            }
        }

        private bool IsActive(TimelineEvent e)
        {
            return _currentTime >= e.time && _currentTime <= e.time + e.duration;
        }
    }
}