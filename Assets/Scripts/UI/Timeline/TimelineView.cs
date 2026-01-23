using System;
using UnityEngine;

namespace DefqonEngine.UI.Timeline
{
    public class TimelineView : MonoBehaviour
    {
        [Header("View")]
        public RectTransform panel;

        [Header("Zoom")]
        public float pixelsPerSecond = 100f;

        [Header("Scroll")]
        public float scrollTime = 0f;

        [Header("Audio")]
        public AudioSource audioSource;


        public Action OnViewChanged;

        public float Width => panel.rect.width;

        public float TimeToX(float time)
        {
            return (time - scrollTime) * pixelsPerSecond;
        }

        public float XToTime(float x)
        {
            return (x / pixelsPerSecond) + scrollTime;
        }

        public void SetScrollTime(float newScrollTime)
        {
            if (audioSource == null || audioSource.clip == null)
            {
                newScrollTime = Mathf.Max(0f, newScrollTime);
            }
            else
            {
                // Max scroll = clip length - visible timeline
                float maxScroll = Mathf.Max(0f, audioSource.clip.length - (Width / pixelsPerSecond));
                newScrollTime = Mathf.Clamp(newScrollTime, 0f, maxScroll);
            }

            if (Mathf.Abs(scrollTime - newScrollTime) > 0.0001f)
            {
                scrollTime = newScrollTime;
                OnViewChanged?.Invoke();
            }
        }


        public void PanPixels(float deltaX)
        {
            float deltaTime = deltaX / pixelsPerSecond;
            SetScrollTime(scrollTime - deltaTime);
        }

        public void Zoom(float factor, float mouseX)
        {
            if (pixelsPerSecond <= 0f) return;

            float timeUnderMouse = XToTime(mouseX);

            // Nieuwe pixelsPerSecond
            float newPPS = pixelsPerSecond * factor;

            // Clamp: niet verder uitzoomen dan de clip
            if (audioSource != null && audioSource.clip != null)
            {
                float minPPS = Width / audioSource.clip.length;
                newPPS = Mathf.Max(newPPS, minPPS);
            }

            pixelsPerSecond = Mathf.Clamp(newPPS, 20f, 600f); // optioneel max zoom in

            // Pas scrollTime aan zodat de tijd onder de muis blijft
            float newScrollTime = timeUnderMouse - (mouseX / pixelsPerSecond);
            SetScrollTime(newScrollTime);
        }

    }
}
