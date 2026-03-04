using DefqonEngine.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineEventViewManager : MonoBehaviour
    {
        public static TimelineEventViewManager Instance { get; private set; }

        private Dictionary<TimelineEvent, TimelineEventView> views = new();

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
            TimelineEventManager.Instance.OnEventRemovedSpecified += OnEventRemovedSpecified;
            TimelineEventManager.Instance.OnEventSelected += OnEventSelected;
            TimelineEventManager.Instance.OnEventDeselected += OnEventDeselected;
        }


        void OnDisable()
        {
            if (TimelineEventManager.Instance != null)
            {
                TimelineEventManager.Instance.OnEventAdded -= OnEventAdded;
                TimelineEventManager.Instance.OnEventRemoved -= OnEventRemoved;
                TimelineEventManager.Instance.OnEventRemovedSpecified -= OnEventRemovedSpecified;
                TimelineEventManager.Instance.OnEventSelected -= OnEventSelected;
                TimelineEventManager.Instance.OnEventDeselected -= OnEventDeselected;
            }
        }


        private void OnEventAdded(TimelineEvent ev)
        {
            TimelineTrack track = TimelineTrackManager.Instance.FindTrackByIndex(ev.trackIndex);
            if (track == null)
            {
                Debug.LogWarning($"Track {ev.trackIndex} niet gevonden voor event.");
                return;
            }

            TimelineEventView view = Instantiate(eventPrefab, eventsLayer);
            view.Initialize(ev, track);
            views[ev] = view;
            TimelineEventManager.Instance.SelectEvent(ev);
        }
        private void OnEventRemoved()
        {
            if (selectedView == null)
                return;

            Destroy(selectedView.gameObject);
            selectedView = null;
        }
        private void OnEventRemovedSpecified(TimelineEvent timelineEvent)
        {
            if (!views.TryGetValue(timelineEvent, out var view))
            {
                Debug.LogWarning("View not found for event removal.");
                return;
            }
            Destroy(view.gameObject);
            views.Remove(timelineEvent);
        }

        public void OnEventSelected(TimelineEvent timelineEvent)
        {

            if (selectedView != null)
            {
                selectedView.Deselect();
            }
            selectedView = views[timelineEvent];
            selectedView.Select();
        }

        public void OnEventDeselected()
        {
            if (selectedView != null)
            {
                selectedView.Deselect();
            }
            selectedView = null;
        }
    }
}
