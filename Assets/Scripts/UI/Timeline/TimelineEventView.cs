using UnityEngine;
using DefqonEngine.Lighting.Data;

namespace DefqonEngine.UI.Timeline
{
    public class TimelineEventView : MonoBehaviour
    {
        public LightEvent lightEvent;
        public TimelineView timeline;
        public RectTransform rect;
        public int trackIndex;
        public float trackHeight = 40f;

        void Update()
        {
            Refresh();
        }

        public void Refresh()
        {
            float x = timeline.TimeToX(lightEvent.time);
            float width = timeline.TimeToX(lightEvent.time + lightEvent.duration) - x;
            float y = -trackIndex * trackHeight;
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, trackHeight - 4);
        }
    }
}
