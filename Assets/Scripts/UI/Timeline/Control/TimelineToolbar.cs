using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.UI.Timeline.Common;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineToolbar : MonoBehaviour
    {
        [Header("UI References")]
        public Button addTrackButton;
        public Button removeTrackButton;
        public Button snappingButton;

        [Header("Settings")]
        public Color snappingNoneColor = Color.white;
        public Color snappingTimeColor = Color.blue;
        public Color snappingEventsColor = Color.green;

        void Start()
        {
            addTrackButton.onClick.AddListener(OnAddTrackClicked);
            removeTrackButton.onClick.AddListener(OnRemoveTrackClicked);
            snappingButton.onClick.AddListener(OnSnappingClicked);
        }

        private void OnAddTrackClicked()
        {
            TimelineTrackManager.Instance.AddTrack();
        }

        private void OnRemoveTrackClicked()
        {
            TimelineTrackManager.Instance.RemoveTrack();
        }

        private void OnSnappingClicked()
        {
            TimelineView.Instance.UpdateSnappingMode();
            switch (TimelineView.Instance.snappingMode)
            {
                case SnappingMode.None:
                    snappingButton.image.color = snappingNoneColor;
                    break;
                case SnappingMode.Time:
                    snappingButton.image.color = snappingTimeColor;
                    break;
                case SnappingMode.Events:
                    snappingButton.image.color = snappingEventsColor;
                    break;
            }
        }
    }
}
