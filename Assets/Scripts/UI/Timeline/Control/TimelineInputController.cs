using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Development.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineInputController : MonoBehaviour
    {
        public static TimelineInputController Instance { get; private set; }
        void Awake()
        {
            Instance = this;
        }
        void Update()
        {
            HandleZoom();
            HandlePan();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TimelineAudioController.Instance.TogglePlayPause();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TimelineEventManager.Instance.RemoveEvent();
            }
        }

        public void OnTrackRightClicked(TimelineTrack track, PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local
            );

            float time = Mathf.Max(0f, TimelineView.Instance.XToTime(local.x));

            // stuur naar EventManager
            TimelineEventManager.Instance.CreateEvent<LightEvent>(track.trackIndex, track.lampGroup.id, time);
        }

        void HandleZoom()
        {
            if (!Input.GetKey(KeyCode.LeftControl)) return;
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            Vector2 localMouse;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                Input.mousePosition,
                null,
                out localMouse
            );

            TimelineView.Instance.Zoom(scroll > 0 ? 1.1f : 0.9f, localMouse.x);
        }

        void HandlePan()
        {
            if (Input.GetMouseButton(2))
            {
                float deltaX = Input.GetAxis("Mouse X") * 20f;
                TimelineView.Instance.PanPixels(deltaX);
            }
        }
    }
}
