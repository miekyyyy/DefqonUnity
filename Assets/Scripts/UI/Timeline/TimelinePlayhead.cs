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
        private bool isPlaying;

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
            isPlaying = audioSource.isPlaying;
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

            float x = Mathf.Clamp(local.x + timeline.Width, 0f, timeline.Width);

            // Tijdsberekening
            float time = timeline.scrollTime + (x / timeline.pixelsPerSecond);

            // clamp tijd binnen audio clip
            if (audioSource.clip != null)
                time = Mathf.Clamp(time, 0f, audioSource.clip.length);

            audioSource.time = time;

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }





        public void EndDrag()
        {
            isDragging = false;
            if (isPlaying)
                audioSource.UnPause();
            isPlaying = false;
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
