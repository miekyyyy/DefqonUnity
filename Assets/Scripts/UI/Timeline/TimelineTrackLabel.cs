using TMPro;
using UnityEngine;

namespace DefqonEngine.UI.Timeline
{
    public class TimelineTrackLabel : MonoBehaviour
    {
        public TimelineTrack track;
        public TextMeshProUGUI label;

        void Update()
        {
            if (track)
                label.text = track.GroupName;
        }
    }
}
