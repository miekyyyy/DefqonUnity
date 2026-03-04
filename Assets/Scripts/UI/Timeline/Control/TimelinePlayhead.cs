using DefqonEngine.UI.Timeline.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Timeline.Control
{
    public class TimelinePlayhead : MonoBehaviour
    {
        public RectTransform rect;


        public float scrollMargin = 50f;
        private bool isDragging;
        private bool isPlaying;

        void LateUpdate()
        {
            if (isDragging) return;

            float x = TimelineView.Instance.TimeToX(TimelineAudioController.Instance.GetCurrentTime());
            x = Mathf.Clamp(x, 0f, TimelineView.Instance.Width);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);

            HandleAutoScroll(x);
        }

        public void BeginDrag(BaseEventData eventData)
        {

            isDragging = true; 

            isPlaying = TimelineAudioController.Instance.IsPlaying();
            TimelineAudioController.Instance.Pause();
        }

        public void Drag(BaseEventData eventData)
        {
            if(eventData is not PointerEventData)
                return;
            PointerEventData pointerEventData = (PointerEventData)eventData;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                pointerEventData.position,
                pointerEventData.pressEventCamera,
                out Vector2 local))
                return;

            float x = Mathf.Clamp(local.x, 0f, TimelineView.Instance.Width);

            float time = TimelineView.Instance.XToTime(x);

            TimelineAudioController.Instance.SetTime(time);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }

        public void EndDrag(BaseEventData eventData)
        {
            isDragging = false;
            if (isPlaying)
                TimelineAudioController.Instance.Play();
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
