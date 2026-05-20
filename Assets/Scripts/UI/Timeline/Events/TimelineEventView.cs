using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Tracks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineEventView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
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
            TimelineTrackManager.Instance.OnTrackRemoved += OnTrackRemoved;
            UpdateVisual();
        }

        void OnDestroy()
        {
            if (TimelineView.Instance != null)
                TimelineView.Instance.OnViewChanged -= UpdateVisual;
        }

        void OnTrackRemoved(TimelineTrack removedTrack)
        {
            if (removedTrack == track)
                TimelineEventManager.Instance.RemoveEvent(timelineEvent);
        }

        public void UpdateVisual()
        {
            float x = TimelineView.Instance.TimeToX(timelineEvent.time);
            float xStart = TimelineView.Instance.TimeToX(timelineEvent.time);
            float xEnd = TimelineView.Instance.TimeToX(timelineEvent.time + timelineEvent.duration);
            float w = xEnd - xStart;

            rect.anchoredPosition = new Vector2(x, track.GetTrackY());
            rect.sizeDelta = new Vector2(w, rect.sizeDelta.y);

            UpdateResizeHandles(w);
        }

        private void UpdateResizeHandles(float width)
        {
            bool showVisuals = width >= minWidthForHandles;
            foreach (var handle in resizeHandles)
                handle.SetVisible(showVisuals);
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

            rect.SetAsLastSibling(); // Zorg dat het event boven andere events komt tijdens het slepen
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
            float t = TimelineView.Instance.XToTime(x);

            // Alleen visueel verplaatsen
            timelineEvent.time = t;
            UpdateVisual();
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            // Pas trimmen nadat het event los wordt gelaten, zodat andere events niet direct weg worden gehaald
            TimelineEventManager.Instance.ApplyTrim(timelineEvent, timelineEvent.time);
            UpdateVisual();
        }

        public void Deselect()
        {
            image.color = defaultColor;
            foreach (var item in resizeHandles)
                item.Deselect();
        }

        public void Select()
        {
            image.color = selectedColor;
            foreach (var item in resizeHandles)
                item.Select();
        }
    }
}