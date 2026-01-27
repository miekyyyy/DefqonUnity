using UnityEngine;
using UnityEngine.EventSystems;
using DefqonEngine.Lighting.Data;
using DefqonEngine.UI.Timeline.Common;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineEventView : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public RectTransform rect;
        public TimelineTrack track;
        public LightEvent lightEvent;

        private float dragOffset;

        public float startTime
        {
            get => lightEvent.time;
            set
            {
                lightEvent.time = value;
                UpdateVisual();
            }
        }

        public float duration
        {
            get => lightEvent.duration;
            set
            {
                lightEvent.duration = value;
                UpdateVisual();
            }
        }

        public void Initialize(LightEvent ev, TimelineTrack parentTrack)
        {
            lightEvent = ev;
            track = parentTrack;
            UpdateVisual();
        }

        public void UpdateVisual()
        {
            float x = TimelineView.Instance.TimeToX(startTime);
            float width = duration * TimelineView.Instance.pixelsPerSecond;
            rect.anchoredPosition = new Vector2(x, 0);
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            dragOffset = local.x - TimelineView.Instance.TimeToX(startTime);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            float time = TimelineView.Instance.XToTime(local.x - dragOffset);
            startTime = Mathf.Max(0f, time);
        }
    }
}
