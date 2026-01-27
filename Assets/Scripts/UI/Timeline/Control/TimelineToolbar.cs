using DefqonEngine.Lighting.Groups;
using DefqonEngine.Lighting.Runtime;
using DefqonEngine.UI.Timeline;
using DefqonEngine.UI.Timeline.Development.Events;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelineToolbar : MonoBehaviour
    {
        [Header("UI References")]
        public Button addTrackButton;
        public Button removeTrackButton;
        public Button snappingButton;
        public TMP_Dropdown groupDropdown;

        [Header("Managers")]
        public TimelineTrackManager trackManager;

        private bool dropdownOpen = false;
        private bool adding = false;
        private bool removing = false;

        void Start()
        {
            groupDropdown.gameObject.SetActive(false);

            addTrackButton.onClick.AddListener(OnAddTrackClicked);
            removeTrackButton.onClick.AddListener(OnRemoveTrackClicked);
            snappingButton.onClick.AddListener(OnSnappingClicked);

            groupDropdown.onValueChanged.AddListener(OnDropdownSelected);
        }

        private void OnDropdownSelected(int index)
        {
            if (index <= 0)
                return; // placeholder gekozen → niks doen

            if (adding)
            {
                // index -1 voor placeholder
                int groupIndex = index - 1;
                var groupsToAdd = GetAvailableGroupsForAdd();
                if (groupIndex >= 0 && groupIndex < groupsToAdd.Count)
                {
                    var group = groupsToAdd[groupIndex];
                    if (group != null)
                    {
                        trackManager.AddTrack(group);
                    }
                }
            }
            else if (removing)
            {
                // index -1 voor placeholder
                int trackIndex = index - 1;
                var tracksToRemove = GetTracksForRemove();
                if (trackIndex >= 0 && trackIndex < tracksToRemove.Count)
                {
                    var track = tracksToRemove[trackIndex];
                    if (track != null)
                    {
                        trackManager.RemoveTrack(track);
                    }
                }
            }

            HideDropdown();
        }

        private void OnAddTrackClicked()
        {
            if (dropdownOpen && adding)
            {
                HideDropdown();
                return;
            }

            adding = true;
            removing = false;
            PopulateDropdownForAdd();
            ShowDropdownNearButton(addTrackButton);
        }

        private void OnRemoveTrackClicked()
        {
            if (dropdownOpen && removing)
            {
                HideDropdown();
                return;
            }

            adding = false;
            removing = true;
            PopulateDropdownForRemove();
            ShowDropdownNearButton(removeTrackButton);
        }

        private void OnSnappingClicked()
        {
            Debug.Log("[Toolbar] Snapping clicked (not implemented)");
        }

        private void PopulateDropdownForAdd()
        {
            groupDropdown.ClearOptions();
            List<string> names = new() { "No group selected" };

            var available = GetAvailableGroupsForAdd();
            foreach (var g in available)
                names.Add(g.groupName);

            groupDropdown.AddOptions(names);
            groupDropdown.value = 0;
            groupDropdown.RefreshShownValue();
        }

        private void PopulateDropdownForRemove()
        {
            groupDropdown.ClearOptions();

            List<string> optionNames = new() { "No track selected" };

            foreach (var track in trackManager.Tracks)
                if (track.lampGroup != null)
                    optionNames.Add(track.lampGroup.groupName);

            groupDropdown.AddOptions(optionNames);

            // Zorg dat de dropdown een minimale height heeft zodat hij zichtbaar is
            RectTransform dropdownRect = groupDropdown.GetComponent<RectTransform>();
            dropdownRect.sizeDelta = new Vector2(dropdownRect.sizeDelta.x, Mathf.Max(150f, optionNames.Count * 30f));

            groupDropdown.value = 0;
            groupDropdown.RefreshShownValue();
        }


        private List<LampGroup> GetAvailableGroupsForAdd()
        {
            List<LampGroup> available = new();
            foreach (var entry in LightManager.Instance.groupList)
            {
                if (trackManager.FindTrackFromGroup(entry.group) == null)
                    available.Add(entry.group);
            }
            return available;
        }

        private List<TimelineTrack> GetTracksForRemove()
        {
            return new List<TimelineTrack>(trackManager.Tracks);
        }

        private void ShowDropdownNearButton(Button button)
        {
            RectTransform btnRect = button.GetComponent<RectTransform>();
            RectTransform dropdownRect = groupDropdown.GetComponent<RectTransform>();

            dropdownRect.position = new Vector3(
                btnRect.position.x,
                btnRect.position.y - btnRect.rect.height,
                btnRect.position.z
            );

            groupDropdown.gameObject.SetActive(true);
            dropdownOpen = true;
        }

        private void HideDropdown()
        {
            groupDropdown.gameObject.SetActive(false);
            dropdownOpen = false;
            adding = false;
            removing = false;
        }
    }
}
