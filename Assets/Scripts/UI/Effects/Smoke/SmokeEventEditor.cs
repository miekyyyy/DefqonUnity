using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Stage.Fixtures.Management;
using DefqonEngine.Stage.Fixtures.Smoke;
using UnityEngine;

namespace DefqonEngine.UI.Effects.Smoke
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
