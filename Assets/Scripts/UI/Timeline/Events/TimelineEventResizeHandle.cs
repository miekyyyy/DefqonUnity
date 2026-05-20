using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.UI.Timeline.Common;
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

        float startStartTime;
        float startDuration;
        [Header("Selections")]
        [SerializeField] Image visual;
        [SerializeField] Image hitArea;

        [SerializeField] Color defaultColor;
        [SerializeField] Color selectedColor;

        public void OnBeginDrag(PointerEventData eventData)
        {
            startStartTime = eventView.startTime;
            startDuration = eventView.duration;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
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
                snappedStart = Mathf.Clamp(snappedStart, 0, startStartTime + startDuration - 0.05f);
                
                var prev = TimelineEventManager.Instance.GetPreviousEvent(eventView.timelineEvent);
                if (prev != null)
                {
                    snappedStart = Mathf.Max(snappedStart, prev.time + prev.duration);
                }
                float delta = startStartTime - snappedStart;

                eventView.startTime = snappedStart;
                eventView.duration = startDuration + delta;
            }
            else
            {
                float snappedEnd = TimelineView.Instance.SnapTime(
                    time,
                    SnapContext.ResizeEnd,
                    eventView.timelineEvent
                );

                snappedEnd = Mathf.Max(snappedEnd, startStartTime + 0.05f);

                var next = TimelineEventManager.Instance.GetNextEvent(eventView.timelineEvent);
                if (next != null)
                {
                    float maxEnd = next.time;
                    snappedEnd = Mathf.Min(snappedEnd, maxEnd);
                }

                eventView.duration = snappedEnd - startStartTime;
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
