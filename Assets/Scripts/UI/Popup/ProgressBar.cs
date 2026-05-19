using TMPro;
using UnityEngine;

namespace DefqonEngine.UI.Popup
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform progressBarFill;
        [SerializeField] private TMP_Text progressLabel;

        public void SetProgress(float progress, string label = "Loading...")
        {
            progress = Mathf.Clamp01(progress);
            progressBarFill.localScale = new Vector3(progress, 1f, 1f);
            if(progressLabel != null) 
                progressLabel.text = label;
        }
    }
}
