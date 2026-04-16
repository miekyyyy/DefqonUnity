using DefqonEngine.Lighting.Groups;
using DefqonEngine.UI.Timeline.Control;
using UnityEngine;
using UnityEngine.UI;

public class SmokeEventGroupButton : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text nameText;
    [SerializeField] Button button;
    public void Initialize(SmokeGroup smokeGroup)
    {
        nameText.text = smokeGroup.groupName;
        button.onClick.AddListener(() => SmokeEventEditor.Instance.UpdateSmokeEventGroup(smokeGroup));
    }
}