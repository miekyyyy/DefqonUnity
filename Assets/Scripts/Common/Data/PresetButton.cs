using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.Common.Data
{
    public class PresetButton : MonoBehaviour
    {
        public EventPreset preset;
        [SerializeField] Button button;
        [SerializeField] TMP_Text text;
        public bool isDefault { get; private set; }
        public void Initialize(EventPreset preset, bool isDefault)
        {
            this.preset = preset;
            this.isDefault = isDefault;
            text.text = preset.Name;
            button.onClick.AddListener(() => PresetManager.Instance.SelectPreset(preset));
        }
        public void OnSelect()
        {
            //Highlight the button
            button.interactable = false;
        }

        public void OnDeselect()
        {
            //Unhighlight the button
            button.interactable = true;
        }

        public void StartDeleting()
        {
            if (isDefault) return;
            button.colors = new ColorBlock()
            {
                normalColor = button.colors.normalColor,
                highlightedColor = PresetManager.Instance.removingColor,
                pressedColor = button.colors.pressedColor,
                selectedColor = button.colors.selectedColor,
                disabledColor = button.colors.disabledColor,
                colorMultiplier = button.colors.colorMultiplier,
                fadeDuration = button.colors.fadeDuration
            };
        }

        public void StopDeleting()
        {
            button.colors = new ColorBlock()
            {
                normalColor = button.colors.normalColor,
                highlightedColor = PresetManager.Instance.defaultColor,
                pressedColor = button.colors.pressedColor,
                selectedColor = button.colors.selectedColor,
                disabledColor = button.colors.disabledColor,
                colorMultiplier = button.colors.colorMultiplier,
                fadeDuration = button.colors.fadeDuration
            };
        }
    }
}
