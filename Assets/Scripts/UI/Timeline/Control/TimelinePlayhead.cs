using DefqonEngine.Sequencing.Audio;
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

        void Update()
        {
            if (isDragging) return;

            float time = AudioPlaybackController.Instance.GetCurrentTime();

            float x = TimelineView.Instance.TimeToX(time);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);

            TimelineView.Instance.AutoScrollToTime(time, scrollMargin);
        }

        public void BeginDrag(BaseEventData eventData)
        {

            isDragging = true;

            isPlaying = AudioPlaybackController.Instance.IsPlaying();
            AudioPlaybackController.Instance.Pause();
        }

        public void Drag(BaseEventData eventData)
        {
            if (eventData is not PointerEventData)
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

            AudioPlaybackController.Instance.SetTime(time);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }

        public void EndDrag(BaseEventData eventData)
        {
            isDragging = false;
            if (isPlaying)
                AudioPlaybackController.Instance.Play();
            isPlaying = false;
        }

        public void OnClickRuler(BaseEventData eventData)
        {
            if (eventData is not PointerEventData)
                return;
            BeginDrag(eventData);
            Drag(eventData);
            EndDrag(eventData);
        }

    }
}
