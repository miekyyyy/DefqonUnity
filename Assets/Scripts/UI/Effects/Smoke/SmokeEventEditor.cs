using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Stage.Fixtures.Management;
using DefqonEngine.Stage.Fixtures.Smoke;
using DefqonEngine.UI.Effects.Light;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.UI.Effects.Smoke
{
    public class SmokeEventEditor : MonoBehaviour
    {
        public static SmokeEventEditor Instance { get; private set; }
        [Header("Groups")]
        [SerializeField] GameObject groupParent;
        [SerializeField] SmokeEventGroupButton groupButtonPrefab;
        List<SmokeEventGroupButton> groupButtons = new List<SmokeEventGroupButton>();
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
            foreach (var button in groupButtons)
            {
                button.SetSelected(button.smokeGroup.id == timelineEvent.targetId);
            }
        }

        private void LoadGroups()
        {
            var available = SmokeManager.Instance.groupList;
            foreach (var g in available)
            {
                var button = Instantiate(groupButtonPrefab, groupParent.transform);
                button.Initialize(g.group);
                groupButtons.Add(button);
            }
        }

        public void UpdateSmokeEventGroup(SmokeGroup smokeGroup)
        {
            foreach (var button in groupButtons)
            {
                button.SetSelected(button.smokeGroup.id == smokeGroup.id);
                Debug.Log($"Updated button from {button.smokeGroup.id} to {smokeGroup.id}");
            }
            if (TimelineEventManager.Instance.selectedEvents == null)
                return;
            foreach (var ev in TimelineEventManager.Instance.selectedEvents)
            {
                if (ev is not SmokeEvent smokeEvent)
                    return;

                smokeEvent.targetId = smokeGroup.id;
            }
        }
    }
}
