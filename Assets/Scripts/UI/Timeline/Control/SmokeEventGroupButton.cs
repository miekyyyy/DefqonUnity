using DefqonEngine.Fixtures.Smoke;
using UnityEngine;
using UnityEngine.UI;

namespace DefqonEngine.UI.Timeline.Control
{

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
}