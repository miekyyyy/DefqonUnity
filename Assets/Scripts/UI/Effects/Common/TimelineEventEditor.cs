using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Effects.Light;
using DefqonEngine.UI.Effects.Smoke;
using System;
using System.Collections.Generic;
using UnityEngine;
using EventType = DefqonEngine.Sequencing.Data.Events.EventType;
namespace DefqonEngine.UI.Effects.Common
{
    public class TimelineEventEditor : MonoBehaviour
    {
        [SerializeField] List<EventEditor> eventEditors;

        void OnEnable()
        {
            if (TimelineEventManager.Instance != null)
            {
                TimelineEventManager.Instance.OnEventSelected += OnEventSelected;
                TimelineEventManager.Instance.OnEventDeselected += OnEventDeselected;
            }
        }

        void OnDisable()
        {
            if (TimelineEventManager.Instance != null)
            {
                TimelineEventManager.Instance.OnEventSelected -= OnEventSelected;
                TimelineEventManager.Instance.OnEventDeselected -= OnEventDeselected;
            }
        }

        void OnEventSelected(TimelineEvent timelineEvent)
        {
            UpdateEditor(timelineEvent);
        }
        void OnEventDeselected()
        {
            eventEditors.ForEach(e => e.editorPanel.SetActive(false));
            if(TimelineEventManager.Instance != null && TimelineEventManager.Instance.selectedEvents.Count > 0)
            {
                UpdateEditor(TimelineEventManager.Instance.selectedEvents[TimelineEventManager.Instance.selectedEvents.Count - 1]);
            }
        }

        void UpdateEditor(TimelineEvent timelineEvent)
        {
            EventType eventType = timelineEvent.type;
            eventEditors.ForEach(e => e.editorPanel.SetActive(false));
            GameObject editor = eventEditors.Find(e => e.eventType == eventType).editorPanel;
            editor.SetActive(true);
            switch (eventType)
            {
                case EventType.Light:
                    editor.GetComponent<LightEventEditor>().LoadFromEvent(timelineEvent);
                    break;
                case EventType.Smoke:
                    editor.GetComponent<SmokeEventEditor>().LoadFromEvent(timelineEvent);
                    break;
                default:
                    Debug.LogWarning("[TimelineEventEditor] EventType not implemented yet]");
                    break;
            }
        }
    }

    [Serializable]
    public struct EventEditor
    {
        public EventType eventType;
        public GameObject editorPanel;
    }
}