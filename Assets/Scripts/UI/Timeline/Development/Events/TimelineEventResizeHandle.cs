using DefqonEngine.UI.Timeline.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        [Header("Selections")]
        [SerializeField] Image image;
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
                float newStart = Mathf.Min(time, startStartTime + startDuration - 0.05f);
                var prev = TimelineEventManager.Instance.GetPreviousEvent(eventView.lightEvent);
                if (prev != null)
                {
                    newStart = Mathf.Max(newStart, prev.time + prev.duration);
                }
                float delta = startStartTime - newStart;

                eventView.startTime = newStart;
                eventView.duration = startDuration + delta;
            }
            else
            {
                float newDuration = Mathf.Max(0.05f, time - startStartTime);

                var next = TimelineEventManager.Instance.GetNextEvent(eventView.lightEvent);
                if (next != null)
                {
                    float maxDuration = next.time - eventView.startTime;
                    newDuration = Mathf.Min(newDuration, maxDuration);
                }

                eventView.duration = newDuration;
            }
        }

        public void Deselect()
        {
            image.color = defaultColor;
        }

        public void Select()
        {
            image.color = selectedColor;
        }
    }
}
