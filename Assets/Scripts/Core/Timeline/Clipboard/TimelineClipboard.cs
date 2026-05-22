using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.History;
using DefqonEngine.Sequencing.Audio;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Timeline.Control;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Assets.Scripts.Core.Timeline.Clipboard
{
    public class TimelineClipboard : MonoBehaviour
    {
        public static TimelineClipboard Instance { get; private set; }

        public List<TimelineEvent> copiedEvents;


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
            if (TimelineEventManager.Instance.selectedEvents != null)
            {
                foreach (var timelineEvent in TimelineEventManager.Instance.selectedEvents)
                {
                    copiedEvents.Add(TimelineEventManager.CloneEvent(timelineEvent));
                }
            }
        }

        public void PasteFromClipboard()
        {
            if (copiedEvents != null)
            {
                TimelineHistory.Instance.SaveState("Pasting Event");
                float time = AudioPlaybackController.Instance.GetCurrentTime();
                foreach (var timelineEvent in copiedEvents)
                {
                    TimelineEventManager.Instance.CreateEvent(timelineEvent, time);
                }
            }
        }

    }
}
