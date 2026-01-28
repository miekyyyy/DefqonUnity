using DefqonEngine.Lighting.Data;
using DefqonEngine.UI.Timeline.Common;
using System;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineEventViewManager : MonoBehaviour
    {
        public static TimelineEventViewManager Instance { get; private set; }
        public Transform eventsLayer;           // Parent voor alle event visuals
        public TimelineEventView eventPrefab;   // Prefab voor individuele events

        public TimelineEventView selectedView;

        private void Awake()
        {
            Instance = this;
        }
        void OnEnable()
        {
            TimelineEventManager.Instance.OnEventAdded += OnEventAdded;
            TimelineEventManager.Instance.OnEventRemoved += OnEventRemoved;
        }


        void OnDisable()
        {
            if (TimelineEventManager.Instance != null)
            {
                TimelineEventManager.Instance.OnEventAdded -= OnEventAdded;
                TimelineEventManager.Instance.OnEventRemoved -= OnEventRemoved;
            }
        }

        private void OnEventAdded(LightEvent ev)
        {
            TimelineTrack track = TimelineTrackManager.Instance.FindTrackByIndex(ev.trackIndex);
            if (track == null)
            {
                Debug.LogWarning($"Track {ev.trackIndex} niet gevonden voor event.");
                return;
            }

            TimelineEventView view = Instantiate(eventPrefab, eventsLayer);
            view.Initialize(ev, track);
            SelectEvent(view);
            Debug.Log($"Event toegevoegd: track {ev.trackIndex}, time {ev.time}, op x {((RectTransform)view.transform).anchoredPosition.x} en y {((RectTransform)view.transform).anchoredPosition.y}");
        }
        private void OnEventRemoved()
        {
            Destroy(selectedView.gameObject);
            selectedView = null;
        }

        public void SelectEvent(TimelineEventView timelineEventView)
        {
            if(selectedView != null)
            {
                selectedView.Deselect();
            }
            selectedView = timelineEventView;
            timelineEventView.Select();

        }

        public void DeselectEvent()
        {
            if (selectedView != null)
            {
                selectedView.Deselect();
            }
            selectedView = null;
        }
    }
}
