using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.UI.Timeline.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineTrack : MonoBehaviour, IPointerClickHandler
    {
        public LampGroup lampGroup;
        public int trackIndex;
        public List<LightEvent> events = new();

        [Header("UI")]
        public RectTransform eventsContainer;
        public TimelineEventView eventPrefab;

        public void AddEvent(LightEvent ev)
        {
            ev.trackIndex = trackIndex;
            events.Add(ev);

            if (eventPrefab != null && eventsContainer != null)
            {
                var view = Instantiate(eventPrefab, eventsContainer);
                view.Initialize(ev, this);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            float time = 0f;

            if (TimelineView.Instance != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    TimelineView.Instance.panel,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 local
                );
                time = TimelineView.Instance.XToTime(local.x);
            }

            LightEvent newEvent = new LightEvent
            {
                time = Mathf.Max(0f, time),
                duration = 1f,
                color = Color.white,
                trackIndex = trackIndex,
                targetType = TargetType.Group,
                targetId = lampGroup.id
            };

            AddEvent(newEvent);

            Debug.Log($"[Timeline] Added event to track '{lampGroup.groupName}' at {newEvent.time:F2}s");
        }

        public string GroupName => lampGroup.groupName; // leesbare naam voor labels
    }
}
