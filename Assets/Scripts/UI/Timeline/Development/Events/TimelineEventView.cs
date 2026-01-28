using UnityEngine;
using UnityEngine.EventSystems;
using DefqonEngine.Lighting.Data;
using DefqonEngine.UI.Timeline.Common;
using System;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;

namespace DefqonEngine.UI.Timeline.Development.Events
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
        public TimelineEvent lightEvent;
        public TimelineTrack track;
        public float minDuration = 0.1f;
        [SerializeField] float minWidthForHandles = 80f;

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
                lightEvent.duration = Mathf.Max(value, minDuration);
                UpdateVisual();
            }
        }


        public void Initialize(TimelineEvent ev, TimelineTrack t)
        {
            lightEvent = ev;
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
            float x = TimelineView.Instance.TimeToX(lightEvent.time);
            float w = lightEvent.duration * TimelineView.Instance.pixelsPerSecond;

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
            TimelineEventViewManager.Instance.SelectEvent(this);
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
            lightEvent.time = CheckCollision(TimelineView.Instance.XToTime(x));
            UpdateVisual();
        }

        private float CheckCollision(float candidateTime)
        {
            float start = candidateTime;
            float end = candidateTime + lightEvent.duration;

            foreach (var other in TimelineEventManager.Instance.events)
            {
                if (other == lightEvent)
                    continue;

                if (other.trackIndex != lightEvent.trackIndex)
                    continue;

                float otherStart = other.time;
                float otherEnd = other.time + other.duration;

                if (start < otherEnd && end > otherStart)
                {
                    if (candidateTime > lightEvent.time)
                    {
                        start = otherStart - lightEvent.duration;
                    }
                    else
                    {
                        start = otherEnd;
                    }

                    end = start + lightEvent.duration;
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
