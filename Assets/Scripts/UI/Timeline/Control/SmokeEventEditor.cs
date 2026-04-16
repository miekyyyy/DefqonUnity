using DefqonEngine.Common;
using DefqonEngine.Lighting.Data;
using DefqonEngine.Lighting.Groups;
using DefqonEngine.Lighting.Runtime;
using DefqonEngine.UI.Timeline.Events;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{
    public class SmokeEventEditor : MonoBehaviour
    {
        public static SmokeEventEditor Instance { get; private set; }
        [Header("Groups")]
        [SerializeField] GameObject groupParent;
        [SerializeField] SmokeEventGroupButton groupButton;
        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            LoadGroups();
        }

        public void LoadFromEvent(TimelineEvent timelineEvent)
        {
            Debug.Log("Loading SmokeEvent data into editor...");
        }

        private void LoadGroups()
        {
            var available = SmokeManager.Instance.groupList;
            foreach (var g in available)
            {
                Instantiate(groupButton, groupParent.transform).Initialize(g.group);
            }
        }

        public void UpdateSmokeEventGroup(SmokeGroup smokeGroup)
        {
            if (TimelineEventManager.Instance.selectedEvent == null)
                return;
            if (TimelineEventManager.Instance.selectedEvent is not SmokeEvent smokeEvent)
                return;

            smokeEvent.targetId = smokeGroup.id;
        }
    }
}
