using TMPro;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Tracks
{
    public class TimelineTrackLabel : MonoBehaviour
    {
        public TimelineTrack track;
        public TextMeshProUGUI label;

        void Update()
        {
            if (track)
                label.text = $"Track {track.trackIndex + 1}";
        }
    }
}
