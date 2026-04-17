using DefqonEngine.Fixtures.Light;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{

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
}