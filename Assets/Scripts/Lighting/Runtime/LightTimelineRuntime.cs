using DefqonEngine.Lighting.Data;
using DefqonEngine.UI.Timeline.Development.Events;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Lighting.Runtime
{
    public class LightTimelineRuntime : MonoBehaviour
    {
        public AudioSource audioSource;
        private struct ActiveLightEvent
        {
            public int priority;
            public LightEvent evt;
        }

        void LateUpdate()
        {
            if (audioSource == null) return;

            float currentTime = audioSource.time;

            // Verzamel actieve events per lamp
            Dictionary<int, List<ActiveLightEvent>> perLampEvents = new();

            foreach (var timelineEvent in TimelineEventManager.Instance.events)
            {
                if(timelineEvent is not LightEvent)
                    continue;
                if (!IsActive((LightEvent)timelineEvent, currentTime))
                    continue;

                if (!LightManager.Instance.groups.TryGetValue(timelineEvent.targetId, out var group))
                    continue;

                foreach (var lampId in group.lampIds)
                {
                    if (!perLampEvents.TryGetValue(lampId, out var list))
                    {
                        list = new List<ActiveLightEvent>();
                        perLampEvents[lampId] = list;
                    }

                    list.Add(new ActiveLightEvent
                    {
                        priority = group.priority,
                        evt = (LightEvent)timelineEvent
                    });
                }
            }

            // Evalueren per lamp
            foreach (var lampEntry in LightManager.Instance.lamps)
            {
                Color currentColor = Color.black;

                if (!perLampEvents.TryGetValue(lampEntry.Key, out var events))
                {
                    lampEntry.Value.SetColor(currentColor);
                    continue;
                }

                // van lage naar hoge prioriteit
                events.Sort((a, b) => a.priority.CompareTo(b.priority));

                foreach (var active in events)
                {
                    if (LightEventEvaluator.Evaluate(active.evt, currentTime, currentColor, out Color result))
                    {
                        currentColor = result;
                    }
                }

                lampEntry.Value.SetColor(currentColor);
            }
        }

        // Check of een LightEvent actief is
        private bool IsActive(LightEvent e, float globalTime)
        {
            return globalTime >= e.time && globalTime <= e.time + e.duration;
        }
    }
}
