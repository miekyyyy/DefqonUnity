using DefqonEngine.Core.Timeline.Audio;
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

<<<<<<< HEAD
        void LateUpdate()
        {
            if (isDragging) return;

            float x = TimelineView.Instance.TimeToX(TimelineAudioController.Instance.GetCurrentTime());
            x = Mathf.Clamp(x, 0f, TimelineView.Instance.Width);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);

            HandleAutoScroll(x);
=======
        void Update()
        {
            if (isDragging) return;

            float time = AudioPlaybackController.Instance.GetCurrentTime();

            float x = TimelineView.Instance.TimeToX(time);

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);

            TimelineView.Instance.AutoScrollToTime(time, scrollMargin);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
        }

        public void BeginDrag(BaseEventData eventData)
        {

            isDragging = true; 

<<<<<<< HEAD
            isPlaying = TimelineAudioController.Instance.IsPlaying();
            TimelineAudioController.Instance.Pause();
=======
            isPlaying = AudioPlaybackController.Instance.IsPlaying();
            AudioPlaybackController.Instance.Pause();
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
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

<<<<<<< HEAD
            TimelineAudioController.Instance.SetTime(time);
=======
            AudioPlaybackController.Instance.SetTime(time);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1

            rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
        }

        public void EndDrag(BaseEventData eventData)
        {
            isDragging = false;
            if (isPlaying)
<<<<<<< HEAD
                TimelineAudioController.Instance.Play();
=======
                AudioPlaybackController.Instance.Play();
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
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

<<<<<<< HEAD
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
=======
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
    }
}
