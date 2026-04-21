using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Timeline.Audio;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Sequencing.Data.Presets;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Tracks;
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
                AudioPlaybackController.Instance.TogglePlayPause();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TimelineEventManager.Instance.RemoveEventSelected();
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
            EventPreset selectedPreset = PresetManager.Instance.GetSelectedPreset();
            if (selectedPreset == null || selectedPreset.timelineEvent == null)
            {
                TimelineEventManager.Instance.CreateEvent<LightEvent>(track.trackIndex, 0, time);
                return;
            }
            TimelineEventManager.Instance.CreateEvent(selectedPreset, track.trackIndex, time);
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
