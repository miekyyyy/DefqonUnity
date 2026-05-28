using DefqonEngine.Stage.Fixtures.Smoke;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Effects.Smoke
{

    public class SmokeEventGroupButton : MonoBehaviour
    {
        [SerializeField] TMPro.TMP_Text nameText;
        [SerializeField] Button button;
        public SmokeGroup smokeGroup;
        public void Initialize(SmokeGroup smokeGroup)
        {
            this.smokeGroup = smokeGroup;
            nameText.text = smokeGroup.groupName;
            button.onClick.AddListener(() => SmokeEventEditor.Instance.UpdateSmokeEventGroup(smokeGroup));
        }

        public void SetSelected(bool isSelected)
        {
            button.interactable = !isSelected;
        }
    }
}