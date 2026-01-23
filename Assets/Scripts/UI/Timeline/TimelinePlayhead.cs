using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline
{
    public class TimelinePlayhead : MonoBehaviour
    {
        public RectTransform rect;
        public TimelineView timeline;
        public AudioSource audioSource;

        public float scrollMargin = 50f;
        private bool isDragging;

        void LateUpdate()
        {
            if (isDragging) return;
            if (audioSource == null || audioSource.clip == null) return;

            float x = timeline.TimeToX(audioSource.time);
            x = Mathf.Clamp(x, 0f, timeline.Width);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);

            HandleAutoScroll(x);
        }

        public void BeginDrag()
        {
            isDragging = true;
            audioSource.Pause();
        }

        public void Drag(PointerEventData pointerEventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                timeline.panel,
                pointerEventData.position,
                pointerEventData.pressEventCamera,
                out Vector2 local))
                return;

            // Omdat pivot rechts is, is linkerzijde = -timeline.Width
            float x = Mathf.Clamp(local.x + timeline.Width, 0f, timeline.Width);

            float time = timeline.scrollTime + (x / timeline.pixelsPerSecond);
            audioSource.time = Mathf.Clamp(time, 0f, audioSource.clip.length);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }



        public void EndDrag()
        {
            isDragging = false;
            audioSource.UnPause();
        }

        void HandleAutoScroll(float x)
        {
            float right = timeline.Width - scrollMargin;
            float left = scrollMargin;

            if (x > right)
            {
                float deltaTime = (x - right) / timeline.pixelsPerSecond;
                timeline.SetScrollTime(timeline.scrollTime + deltaTime);
            }
            else if (x < left)
            {
                float deltaTime = (left - x) / timeline.pixelsPerSecond;
                timeline.SetScrollTime(timeline.scrollTime - deltaTime);
            }
        }
    }
}
