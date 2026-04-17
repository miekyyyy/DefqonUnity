using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.UI.Timeline.Control;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Tracks
{
    public class TimelineTrack : MonoBehaviour, IPointerClickHandler
    {
        public int trackIndex;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
            {
                TimelineEventManager.Instance.DeselectEvent();
                return;
            }
            TimelineInputController.Instance.OnTrackRightClicked(this, eventData);
        }

        public float GetTrackY()
        {
            return GetComponent<RectTransform>().anchoredPosition.y;
        }
    }
}
