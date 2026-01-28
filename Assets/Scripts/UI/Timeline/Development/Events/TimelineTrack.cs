
using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.UI.Timeline.Common;
using DefqonEngine.UI.Timeline.Control;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Development.Events
{
    public class TimelineTrack : MonoBehaviour, IPointerClickHandler
    {
        public LampGroup lampGroup;
        public int trackIndex;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
                return;
            TimelineInputController.Instance.OnTrackRightClicked(this, eventData);
        }

        public float GetTrackY()
        {
            return GetComponent<RectTransform>().anchoredPosition.y;
        }

        public string GroupName => lampGroup.groupName; // leesbare naam voor labels
    }
}
