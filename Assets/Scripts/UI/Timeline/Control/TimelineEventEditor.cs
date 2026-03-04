using DefqonEngine.Common;
using DefqonEngine.UI.Timeline.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using EventType = DefqonEngine.Common.EventType;
namespace DefqonEngine.UI.Timeline.Control
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
        }

        void UpdateEditor(TimelineEvent timelineEvent)
        {
            EventType eventType = timelineEvent.type;
            Debug.Log($"[TimelineEventEditor] Updating editor for event type: {eventType}");
            eventEditors.ForEach(e => e.editorPanel.SetActive(false));
            GameObject editor = eventEditors.Find(e => e.eventType == eventType).editorPanel;
            editor.SetActive(true);
            switch (eventType)
            {
                case EventType.Light:
                    editor.GetComponent<LightEventEditor>().LoadFromEvent(timelineEvent);
                    break;
                case EventType.Laser:
                    break;
                case EventType.Servo:
                    break;
                case EventType.Smoke:
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