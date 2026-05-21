using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.History;
using DefqonEngine.Sequencing.Audio;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Timeline.Control;
using UnityEngine;

namespace DefqonEngine.Assets.Scripts.Core.Timeline.Clipboard
{
    public class TimelineClipboard : MonoBehaviour
    {
        public static TimelineClipboard Instance { get; private set; }

        public TimelineEvent copiedEvent;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            TimelineInputController.Instance.OnCopy += CopyToClipboard;
            TimelineInputController.Instance.OnPaste += PasteFromClipboard;
        }

        public void CopyToClipboard()
        {
            if (TimelineEventManager.Instance.selectedEvent != null)
            {
                copiedEvent = TimelineEventManager.CloneEvent(TimelineEventManager.Instance.selectedEvent);
            }
        }

        public void PasteFromClipboard()
        {
            if (copiedEvent != null)
            {
                TimelineHistory.Instance.SaveState("Pasting Event");
                float time = AudioPlaybackController.Instance.GetCurrentTime();
                TimelineEventManager.Instance.CreateEvent(copiedEvent, time);
            }
        }

    }
}
