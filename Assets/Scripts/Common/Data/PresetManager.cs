using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Common.Data
{
    public class PresetManager : MonoBehaviour
    {
        public static PresetManager Instance { get; private set; }
        List<PresetButton> buttons = new List<PresetButton>();
        EventPreset selectedPreset;
        [SerializeField] GameObject parent;
        [SerializeField] PresetButton buttonPrefab;
        [SerializeField] PresetSaveManager saveManager;
        bool isRemovingPreset;

        [Header("Colors")]
        [SerializeField] public Color defaultColor;
        [SerializeField] public Color removingColor;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            // Initialize standard buttons
            AddButton(new EventPreset("Light", new LightEvent()), true);
            AddButton(new EventPreset("Smoke", new SmokeEvent()), true);
        }

        public void AddButton(EventPreset preset, bool isDefault = false)
        {
            var button = Instantiate(buttonPrefab, parent.transform);
            button.Initialize(preset, isDefault);
            buttons.Add(button);
        }

        public void SelectPreset(EventPreset preset)
        {
            foreach (var button in buttons)
            {
                button.OnDeselect();
            }
            selectedPreset = preset;
            var selectedButton = GetPresetButtonFromPreset(preset);
            if (isRemovingPreset)
            {
                RemovePreset();
                return;
            }
            if (selectedButton != null)
            {
                selectedButton.OnSelect();
            }
        }

        public EventPreset GetSelectedPreset()
        {
            return selectedPreset;
        }

        private void RemovePreset()
        {
            if (selectedPreset == null) return;

            var button = GetPresetButtonFromPreset(selectedPreset);

            if (button == null || button.isDefault) return;

            buttons.Remove(button);
            Destroy(button.gameObject);

            selectedPreset = null;
        }

        public void ToggleRemovePresets()
        {
            foreach (var button in buttons)
            {
                button.OnDeselect();
            }
            selectedPreset = null;
            isRemovingPreset = !isRemovingPreset;
            foreach (var button in buttons)
            {
                if (isRemovingPreset)
                {
                    button.StartDeleting();
                }
                else
                {
                    button.StopDeleting();
                }
            }
        }

        private PresetButton GetPresetButtonFromPreset(EventPreset preset)
        {
            foreach (var button in buttons)
            {
                if (button.preset == preset)
                {
                    return button;
                }
            }
            return null;
        }
    }
}
