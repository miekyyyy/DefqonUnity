using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Stage.Fixtures.Light;
using DefqonEngine.Stage.Fixtures.Management;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Effects.Light
{
    public class LightEventEditor : MonoBehaviour
    {
        public static LightEventEditor Instance { get; private set; }

        [Header("Display")]
        [SerializeField] TMP_Text valueRText;
        [SerializeField] TMP_Text valueGText;
        [SerializeField] TMP_Text valueBText;
        [SerializeField] Image colorDisplay;

        [Header("Input")]
        [SerializeField] Slider valueRInput;
        [SerializeField] Slider valueGInput;
        [SerializeField] Slider valueBInput;

        [Header("Groups")]
        [SerializeField] GameObject groupParent;
        [SerializeField] LightEventGroupButton groupButtonPrefab;
        List<LightEventGroupButton> groupButtons = new List<LightEventGroupButton>();

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

        public void SetDisplayR(float r)
        {
            SetDisplay(valueRText, r);
        }
        public void SetDisplayG(float g)
        {
            SetDisplay(valueGText, g);
        }
        public void SetDisplayB(float b)
        {
            SetDisplay(valueBText, b);
        }

        private void LoadGroups()
        {
            var available = LightManager.Instance.groupList;
            foreach (var g in available)
            {
                var button = Instantiate(groupButtonPrefab, groupParent.transform);
                button.Initialize(g.group);
                groupButtons.Add(button);
            }
        }

        private void SetDisplay(TMP_Text targetText, float value)
        {
            targetText.text = value.ToString("0");
            UpdateColorDisplay();
            UpdateLightEventColor();
        }
        private void UpdateColorDisplay()
        {
            colorDisplay.color = new Color(valueRInput.value / 255f, valueGInput.value / 255f, valueBInput.value / 255f);
        }

        private void UpdateLightEventColor()
        {
            if (TimelineEventManager.Instance.selectedEvents == null)
                return;

            foreach (var ev in TimelineEventManager.Instance.selectedEvents)
            {
                if (ev is not LightEvent lightEvent)
                    return;

                Color newColor = new Color(
                    valueRInput.value / 255f,
                    valueGInput.value / 255f,
                    valueBInput.value / 255f
                );

                lightEvent.color = newColor;
            }
        }
        public void LoadFromEvent(TimelineEvent timelineEvent)
        {
            if (timelineEvent is not LightEvent lightEvent)
            {
                Debug.LogWarning("Selected event is not a LightEvent, cannot load color data.");
                return;
            }

            Color c = lightEvent.color;

            valueRInput.value = c.r * 255f;
            valueGInput.value = c.g * 255f;
            valueBInput.value = c.b * 255f;

            foreach (var button in groupButtons)
            {
                button.SetSelected(button.lampGroup.id == timelineEvent.targetId);
            }

            UpdateColorDisplay();
        }

        public void UpdateLightEventGroup(LampGroup lampGroup)
        {
            if (TimelineEventManager.Instance.selectedEvents == null)
                return;

            foreach (var button in groupButtons)
            {
                button.SetSelected(button.lampGroup.id == lampGroup.id);
            }
            foreach (var ev in TimelineEventManager.Instance.selectedEvents)
            {
                if (ev is not LightEvent lightEvent)
                    return;

                lightEvent.targetId = lampGroup.id;
            }
        }
    }
}
