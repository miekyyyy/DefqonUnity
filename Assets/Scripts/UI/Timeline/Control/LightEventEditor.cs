using DefqonEngine.Common;
using DefqonEngine.Lighting.Data;
using DefqonEngine.UI.Timeline.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{
    public class LightEventEditor : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] TMP_Text valueRText;
        [SerializeField] TMP_Text valueGText;
        [SerializeField] TMP_Text valueBText;
        [SerializeField] Image colorDisplay;

        [Header("Input")]
        [SerializeField] Slider valueRInput;
        [SerializeField] Slider valueGInput;
        [SerializeField] Slider valueBInput;

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

        private void SetDisplay(TMP_Text targetText, float value)
        {
            targetText.text = value.ToString("0");
            UpdateColor();
            UpdateLightEvent();
        }
        private void UpdateColor()
        {
            colorDisplay.color = new Color(valueRInput.value / 255f, valueGInput.value / 255f, valueBInput.value / 255f);
        }

        private void UpdateLightEvent()
        {
            if (TimelineEventViewManager.Instance.selectedView == null)
                return;

            if (TimelineEventViewManager.Instance.selectedView.timelineEvent is not LightEvent lightEvent)
                return;

            Color newColor = new Color(
                valueRInput.value / 255f,
                valueGInput.value / 255f,
                valueBInput.value / 255f
            );

            lightEvent.color = newColor;
        }
        public void LoadFromEvent(TimelineEvent timelineEvent)
        {
            Debug.Log("Loading LightEvent data into editor...");

            if (timelineEvent is not LightEvent lightEvent)
            {
                Debug.LogWarning("Selected event is not a LightEvent, cannot load color data.");
                return;
            }

            Debug.Log("LightEvent found, loading color data...");
            Color c = lightEvent.color;

            valueRInput.value = c.r * 255f;
            Debug.Log($"Loaded R: {valueRInput.value}");
            valueGInput.value = c.g * 255f;
            Debug.Log($"Loaded G: {valueGInput.value}");
            valueBInput.value = c.b * 255f;
            Debug.Log($"Loaded B: {valueBInput.value}");

            //Force Sliders to update their display values
            UpdateColor();
        }
    }
}
