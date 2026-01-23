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
            newScrollTime = Mathf.Max(0f, newScrollTime);

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

            pixelsPerSecond = Mathf.Clamp(pixelsPerSecond * factor, 20f, 600f);

            float newScrollTime = timeUnderMouse - (mouseX / pixelsPerSecond);
            SetScrollTime(newScrollTime);
        }
    }
}
