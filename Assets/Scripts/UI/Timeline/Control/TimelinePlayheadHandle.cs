using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelinePlayheadHandle : MonoBehaviour
    {
        public TimelinePlayhead playhead;

        public void OnBeginDrag(BaseEventData eventData)
        {
            playhead.BeginDrag();
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData pointerEventData = (PointerEventData)eventData;
            playhead.Drag(pointerEventData);
        }

        public void OnEndDrag(BaseEventData eventData)
        {
            playhead.EndDrag();
        }
    }
}
