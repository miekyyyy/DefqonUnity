using DefqonEngine.UI.Timeline.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public enum ResizeSide { Left, Right }

    public class TimelineEventResizeHandle : MonoBehaviour,
        IBeginDragHandler, IDragHandler
    {
        public TimelineEventView eventView;
        public ResizeSide side;

        float startStartTime;
        float startDuration;

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
                float newStart = Mathf.Min(time, startStartTime + startDuration - 0.05f);
                float delta = startStartTime - newStart;

                eventView.startTime = newStart;
                eventView.duration = startDuration + delta;
            }
            else
            {
                eventView.duration = Mathf.Max(0.05f, time - startStartTime);
            }
        }
    }
}
