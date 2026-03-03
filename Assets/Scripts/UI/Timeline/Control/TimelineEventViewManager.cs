using DefqonEngine.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineEventViewManager : MonoBehaviour
    {
        public static TimelineEventViewManager Instance { get; private set; }

        public List<TimelineEventView> views = new();

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
        }
         

        void OnDisable()
        {
            if (TimelineEventManager.Instance != null)
            {
                TimelineEventManager.Instance.OnEventAdded -= OnEventAdded;
                TimelineEventManager.Instance.OnEventRemoved -= OnEventRemoved;
                TimelineEventManager.Instance.OnEventRemovedSpecified -= OnEventRemovedSpecified;
                TimelineEventManager.Instance.OnEventSelected -= OnEventSelected;
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
            OnEventSelected(view.timelineEvent);
        }
        
        public TimelineEventView GetViewFromEvent(TimelineEvent timelineEvent)
        {
            return views.FirstOrDefault(view => view.timelineEvent == timelineEvent);
        }

        private void OnEventRemoved()
        {
            Destroy(selectedView.gameObject);
            selectedView = null;
        }
        private void OnEventRemovedSpecified(TimelineEvent timelineEvent)
        {
            Destroy(GetViewFromEvent(timelineEvent).gameObject);
        }

        public void OnEventSelected(TimelineEvent timelineEvent)
        {

            if (selectedView != null)
            {
                selectedView.Deselect();
            }
            selectedView = GetViewFromEvent(timelineEvent);
            selectedView.Select();
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
