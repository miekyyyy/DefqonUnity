using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Audio;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Sequencing.Data.Presets;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Tracks;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineInputController : MonoBehaviour
    {
        public static TimelineInputController Instance { get; private set; }

        public event Action<ZoomData> OnZoom;
        public event Action<float> OnPan;
        public event Action OnCopy;
        public event Action OnPaste;
        public event Action OnUndo;
        public event Action OnRedo;
        public event Action OnMoveUp;
        public event Action OnMoveDown;
        public event Action OnControlDown;
        public event Action OnControlUp;

        public bool isMouseOverUI;
        public bool isControlPressed;

        public bool isTyping;

        void Awake()
        {
            Instance = this;
        }
        void Update()
        {
            HandleZoom();
            HandlePan();

            if (isTyping) return;

            if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                OnControlDown?.Invoke();
            }
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
            {
                OnControlUp?.Invoke();
            }
            isControlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AudioPlaybackController.Instance.TogglePlayPause();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TimelineEventManager.Instance.RemoveEventSelected();
            }
            if (isControlPressed && Input.GetKeyDown(KeyCode.C))
            {
                OnCopy?.Invoke();
            }
            if (isControlPressed && Input.GetKeyDown(KeyCode.V))
            {
                OnPaste?.Invoke();
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z))
            {
                OnUndo?.Invoke();
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Y))
            {
                OnRedo?.Invoke();
            }
            if(isControlPressed && Input.GetKeyDown(KeyCode.UpArrow))
            {
                OnMoveUp?.Invoke();
            }
            if(isControlPressed && Input.GetKeyDown(KeyCode.DownArrow))
            {
                OnMoveDown?.Invoke();
            }
        }

        public void SetTyping(bool typing) => isTyping = typing;

        public void OnTrackRightClicked(TimelineTrack track, PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.viewport,
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
            TimelineEventManager.Instance.CreateEvent(selectedPreset, track.trackIndex, time, selectedPreset.timelineEvent.duration);
        }

        void HandleZoom()
        {
            if (!Input.GetKey(KeyCode.LeftControl)) return;
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            Vector2 localMouse;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.viewport,
                Input.mousePosition,
                null,
                out localMouse
            );
            OnZoom?.Invoke(new ZoomData
            {
                zoomFactor = scroll > 0 ? 1.1f : 0.9f,
                zoomCenterX = localMouse.x
            });
        }

        void HandlePan()
        {
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                float deltaX = Input.mouseScrollDelta.y * 100f;
                OnPan?.Invoke(deltaX);
            }
        }

        public struct ZoomData
        {
            public float zoomFactor;
            public float zoomCenterX;
        }
    }
}
