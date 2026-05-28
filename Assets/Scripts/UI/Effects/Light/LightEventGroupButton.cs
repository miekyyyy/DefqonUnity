using DefqonEngine.Stage.Fixtures.Light;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Effects.Light
{

    public class LightEventGroupButton : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text nameText;
        [SerializeField] Button button;
        public LampGroup lampGroup;
        public void Initialize(LampGroup lampGroup)
        {
            this.lampGroup = lampGroup;
            nameText.text = lampGroup.groupName;
            button.onClick.AddListener(() => LightEventEditor.Instance.UpdateLightEventGroup(lampGroup));
        }

        public void SetSelected(bool isSelected)
        {
            button.interactable = !isSelected;
        }
    }
}