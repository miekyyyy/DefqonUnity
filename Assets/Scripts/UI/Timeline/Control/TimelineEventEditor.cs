using DefqonEngine.Common;
using DefqonEngine.UI.Timeline.Events;
using UnityEngine;
using EventType = DefqonEngine.Common.EventType;
namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineEventEditor : MonoBehaviour
    {
        void Awake()
        {
            TimelineEventManager.Instance.OnEventSelected += OnEventSelected;
        }

        void OnEventSelected(TimelineEvent timelineEvent)
        {
            UpdateEditor(timelineEvent.type);
        }

        void UpdateEditor(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Light:
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
}