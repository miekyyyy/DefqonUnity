using UnityEngine;

namespace DefqonEngine.UI.Timeline.Common
{
    public class CentralizedTimeline : MonoBehaviour
    {
        [SerializeField] private Transform content;

        void Scroll()
        {
            if (TimelineView.Instance == null) return;
            float centerTime = TimelineView.Instance.VisibleStart + 
                (TimelineView.Instance.VisibleEnd - TimelineView.Instance.VisibleStart) / 2f;
            float centerX = TimelineView.Instance.TimeToX(centerTime);
            content.localPosition = new Vector3(-centerX, content.localPosition.y, content.localPosition.z);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}