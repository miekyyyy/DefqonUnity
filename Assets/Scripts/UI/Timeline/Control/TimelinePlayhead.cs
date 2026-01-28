using DefqonEngine.UI.Timeline.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelinePlayhead : MonoBehaviour
    {
        public RectTransform rect;
        public AudioSource audioSource;


        public float scrollMargin = 50f;
        private bool isDragging;
        private bool isPlaying;

        void LateUpdate()
        {
            if (isDragging) return;
            if (audioSource == null || audioSource.clip == null) return;

            float x = TimelineView.Instance.TimeToX(audioSource.time);
            x = Mathf.Clamp(x, 0f, TimelineView.Instance.Width);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);

            HandleAutoScroll(x);
        }

        public void BeginDrag()
        {

            isDragging = true; 
            isPlaying = audioSource != null && audioSource.isPlaying;
            if (audioSource != null)
                audioSource.Pause();

        }

        public void Drag(PointerEventData pointerEventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                pointerEventData.position,
                pointerEventData.pressEventCamera,
                out Vector2 local))
                return;

            float x = Mathf.Clamp(local.x, 0f, TimelineView.Instance.Width);

            float time = TimelineView.Instance.XToTime(x);

            if (audioSource.clip != null)
                time = Mathf.Clamp(time, 0f, audioSource.clip.length);

            audioSource.time = time;

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }

        public void EndDrag()
        {
            isDragging = false;
            if (isPlaying && audioSource != null)
                audioSource.UnPause();
            isPlaying = false;
        }

        void HandleAutoScroll(float x)
        {
            float right = TimelineView.Instance.Width - scrollMargin;
            float left = scrollMargin;

            if (x > right)
            {
                float deltaTime = (x - right) / TimelineView.Instance.pixelsPerSecond;
                TimelineView.Instance.SetScrollTime(TimelineView.Instance.scrollTime + deltaTime);
            }
            else if (x < left)
            {
                float deltaTime = (left - x) / TimelineView.Instance.pixelsPerSecond;
                TimelineView.Instance.SetScrollTime(TimelineView.Instance.scrollTime - deltaTime);
            }
        }
    }
}
