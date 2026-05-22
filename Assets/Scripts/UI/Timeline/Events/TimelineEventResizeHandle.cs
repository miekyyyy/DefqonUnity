using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.History;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Timeline.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Events
{
    public enum ResizeSide { Left, Right }

    public class TimelineEventResizeHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler
    {
        public TimelineEventView eventView;
        public ResizeSide side;

        List<float> startStartTimes = new();
        List<float> startDurations = new();
        float currentEventStartTime;
        float currentEventDuration;
        [Header("Selections")]
        [SerializeField] Image visual;
        [SerializeField] Image hitArea;

        [SerializeField] Color defaultColor;
        [SerializeField] Color selectedColor;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!TimelineEventManager.Instance.selectedEvents.Contains(eventView.timelineEvent))
            {
                TimelineEventManager.Instance.SelectEvent(eventView.timelineEvent);
            }
            TimelineHistory.Instance.SaveState("Resizing Event");

            currentEventStartTime = eventView.timelineEvent.time;
            currentEventDuration = eventView.timelineEvent.duration;

            startStartTimes.Clear();
            startDurations.Clear();

            foreach (var ev in TimelineEventManager.Instance.selectedEvents)
            {
                startStartTimes.Add(ev.time);
                startDurations.Add(ev.duration);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.viewport,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            float time = TimelineView.Instance.XToTime(local.x);

            if (side == ResizeSide.Left)
            {
                float snappedStart = TimelineView.Instance.SnapTime(
                    time,
                    SnapContext.ResizeStart,
                    eventView.timelineEvent
                );
                snappedStart = Mathf.Clamp(snappedStart, 0, currentEventStartTime + currentEventDuration - 0.05f);

                var prev = TimelineEventManager.Instance.GetPreviousEvent(eventView.timelineEvent);
                if (prev != null)
                {
                    snappedStart = Mathf.Max(snappedStart, prev.time + prev.duration);
                }
                float delta = currentEventStartTime - snappedStart;

                for (int i = 0; i < TimelineEventManager.Instance.selectedEvents.Count; i++)
                {
                    TimelineEvent ev = TimelineEventManager.Instance.selectedEvents[i];
                    TimelineEventView evView = TimelineEventManager.Instance.GetView(ev);
                    evView.startTime = snappedStart;
                    evView.duration = startDurations[i] + delta;
                }
            }
            else
            {
                float snappedEnd = TimelineView.Instance.SnapTime(
                    time,
                    SnapContext.ResizeEnd,
                    eventView.timelineEvent
                );

                snappedEnd = Mathf.Max(snappedEnd, currentEventStartTime + 0.05f);

                var next = TimelineEventManager.Instance.GetNextEvent(eventView.timelineEvent);
                if (next != null)
                {
                    float maxEnd = next.time;
                    snappedEnd = Mathf.Min(snappedEnd, maxEnd);
                }

                eventView.duration = snappedEnd - currentEventStartTime;
                float diff = eventView.duration - currentEventDuration;
                for(int i = 0; i < TimelineEventManager.Instance.selectedEvents.Count; i++)
                {
                    TimelineEvent ev = TimelineEventManager.Instance.selectedEvents[i];
                    TimelineEventView evView = TimelineEventManager.Instance.GetView(ev);
                    evView.duration = startDurations[i] + diff;
                }
            }
        }

        public void Deselect()
        {
            visual.color = defaultColor;
        }

        public void Select()
        {
            visual.color = selectedColor;
        }

        public void SetVisible(bool visible)
        {
            visual.enabled = visible;
        }

    }
}
