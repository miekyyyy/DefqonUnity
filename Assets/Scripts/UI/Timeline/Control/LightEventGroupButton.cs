using DefqonEngine.Lighting.Groups;
using DefqonEngine.Lighting.Runtime;
using DefqonEngine.UI.Timeline.Control;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightEventGroupButton : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text nameText;
    [SerializeField] Button button;
    public void Initialize(LampGroup lampGroup)
    {
        nameText.text = lampGroup.groupName;
        button.onClick.AddListener(() => LightEventEditor.Instance.UpdateLightEventGroup(lampGroup));
    }
}