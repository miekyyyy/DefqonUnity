using DefqonEngine.Common;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineEventView : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerClickHandler
    {
        [Header("UI")]
        public RectTransform rect;
        public List<TimelineEventResizeHandle> resizeHandles;
        [SerializeField] Image image;
        [SerializeField] Color defaultColor;
        [SerializeField] Color selectedColor;
        [Header("Settings")]
        public TimelineEvent timelineEvent;
        public TimelineTrack track;
        public float minDuration = 0.1f;
        [SerializeField] float minWidthForHandles = 80f;

        private float dragOffset;

        public float startTime
        {
            get => timelineEvent.time;
            set
            {
                timelineEvent.time = value;
                UpdateVisual();
            }
        }

        public float duration
        {
            get => timelineEvent.duration;
            set
            {
                timelineEvent.duration = Mathf.Max(value, minDuration);
                UpdateVisual();
            }
        }


        public void Initialize(TimelineEvent ev, TimelineTrack t)
        {
            timelineEvent = ev;
            track = t;
            TimelineView.Instance.OnViewChanged += UpdateVisual;
            UpdateVisual();
        }

        void OnDestroy()
        {
            if (TimelineView.Instance != null)
                TimelineView.Instance.OnViewChanged -= UpdateVisual;
        }

        public void UpdateVisual()
        {
            float x = TimelineView.Instance.TimeToX(timelineEvent.time);
            float w = timelineEvent.duration * TimelineView.Instance.pixelsPerSecond;

            rect.anchoredPosition = new Vector2(x, track.GetTrackY());
            rect.sizeDelta = new Vector2(w, rect.sizeDelta.y);

            UpdateResizeHandles(w);
        }
        private void UpdateResizeHandles(float width)
        {
            bool showVisuals = width >= minWidthForHandles;

            foreach (var handle in resizeHandles)
            {
                handle.SetVisible(showVisuals);
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            TimelineEventManager.Instance.SelectEvent(this.timelineEvent);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            dragOffset = local.x - rect.anchoredPosition.x;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            float x = Mathf.Max(0f, local.x - dragOffset);
            timelineEvent.time = CheckCollision(TimelineView.Instance.XToTime(x));
            UpdateVisual();
        }

        private float CheckCollision(float candidateTime)
        {
            float start = candidateTime;
            float end = candidateTime + timelineEvent.duration;

            foreach (var other in TimelineEventManager.Instance.events)
            {
                if (other == timelineEvent)
                    continue;

                if (other.trackIndex != timelineEvent.trackIndex)
                    continue;

                float otherStart = other.time;
                float otherEnd = other.time + other.duration;

                if (start < otherEnd && end > otherStart)
                {
                    if (candidateTime > timelineEvent.time)
                    {
                        start = otherStart - timelineEvent.duration;
                    }
                    else
                    {
                        start = otherEnd;
                    }

                    end = start + timelineEvent.duration;
                }
            }

            return Mathf.Max(0f, start);
        }


        public void Deselect()
        {
            image.color = defaultColor;
            foreach (var item in resizeHandles)
            {
                item.Deselect();
            }
        }
        public void Select()
        {
            image.color = selectedColor;
            foreach (var item in resizeHandles)
            {
                item.Select();
            }
        }
    }
}
