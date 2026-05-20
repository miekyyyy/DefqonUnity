<<<<<<< HEAD
﻿using System;
=======
﻿using DefqonEngine.Core.Timeline.Audio;
using System;
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Common
{
    public class TimelineView : MonoBehaviour
    {

        public static TimelineView Instance { get; private set; }
        [Header("View")]
        public RectTransform panel;

        [Header("Zoom")]
<<<<<<< HEAD
        public float pixelsPerSecond = 100f;
=======
        private float pixelsPerSecond = 100f;
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1

        [Header("Scroll")]
        public float scrollTime = 0f;

<<<<<<< HEAD
        [Header("Audio")]
        public AudioSource audioSource;

=======
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1

        public Action OnViewChanged;

        public float Width => panel.rect.width;

        private void Awake()
        {
            Instance = this;
        }

<<<<<<< HEAD
=======
        // The time currently at the left edge of the timeline
        public float VisibleStart => scrollTime;

        // The time currently at the right edge of the timeline
        public float VisibleEnd => scrollTime + VisibleDuration;

        // How many seconds are visible in the current view
        float VisibleDuration
        {
            get
            {
                if (pixelsPerSecond <= 0f) return 0f;
                return Width / pixelsPerSecond;
            }
        }

        // Time in seconds to X position in pixels, relative to the left edge of the timeline
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
        public float TimeToX(float time)
        {
            return (time - scrollTime) * pixelsPerSecond;
        }

<<<<<<< HEAD
=======
        // X position in pixels to time in seconds, relative to the left edge of the timeline
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
        public float XToTime(float x)
        {
            return (x / pixelsPerSecond) + scrollTime;
        }

<<<<<<< HEAD
        public void SetScrollTime(float newScrollTime)
        {
            if (audioSource == null || audioSource.clip == null)
=======
        // Duration in seconds to width in pixels
        public float DurationToWidth(float duration)
        {
            return TimeToX(scrollTime + duration) - TimeToX(scrollTime);
        }

        // Width in pixels to duration in seconds
        public float WidthToDuration(float width)
        {
            return width / pixelsPerSecond;
        }

        // Time range to width in pixels
        public float TimeRangeToWidth(float start, float end)
        {
            return TimeToX(end) - TimeToX(start);
        }

        public float TimeToSample(float time)
        {
            var clip = AudioPlaybackController.Instance.GetAudioClip();
            if (clip == null) return 0f;

            return time * clip.frequency;
        }

        public float SampleToTime(float sample)
        {
            var clip = AudioPlaybackController.Instance.GetAudioClip();
            if (clip == null || clip.frequency <= 0f) return 0f;

            return sample / clip.frequency;
        }

        public void AutoScrollToTime(float time, float marginPixels)
        {
            float marginTime = marginPixels / pixelsPerSecond;
            float visibleDuration = VisibleDuration;

            float visibleStart = scrollTime;
            float visibleEnd = visibleStart + visibleDuration;

            if (time > visibleEnd - marginTime)
            {
                SetScrollTime(time - visibleDuration + marginTime);
            }
            else if (time < visibleStart + marginTime)
            {
                SetScrollTime(time - marginTime);
            }
        }

        public void SetScrollTime(float newScrollTime)
        {
            if (AudioPlaybackController.Instance == null || AudioPlaybackController.Instance.GetAudioClip() == null)
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
            {
                newScrollTime = Mathf.Max(0f, newScrollTime);
            }
            else
            {
                // Max scroll = clip length - visible timeline
<<<<<<< HEAD
                float maxScroll = Mathf.Max(0f, audioSource.clip.length - (Width / pixelsPerSecond));
=======
                float maxScroll = Mathf.Max(0f, AudioPlaybackController.Instance.GetAudioClip().length - (Width / pixelsPerSecond));
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
                newScrollTime = Mathf.Clamp(newScrollTime, 0f, maxScroll);
            }
            scrollTime = newScrollTime;
            OnViewChanged?.Invoke();
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
<<<<<<< HEAD
            if (audioSource != null && audioSource.clip != null)
            {
                float minPPS = Width / audioSource.clip.length;
=======
            if (AudioPlaybackController.Instance != null && AudioPlaybackController.Instance.GetAudioClip() != null)
            {
                float minPPS = Width / AudioPlaybackController.Instance.GetAudioClip().length;
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
                newPPS = Mathf.Max(newPPS, minPPS);
            }

            pixelsPerSecond = Mathf.Clamp(newPPS, 20f, 600f); // optioneel max zoom in

            // Pas scrollTime aan zodat de tijd onder de muis blijft
            float newScrollTime = timeUnderMouse - (mouseX / pixelsPerSecond);
            SetScrollTime(newScrollTime);
        }

<<<<<<< HEAD
=======
        private void FixedUpdate()
        {
            Debug.Log($"Audio: {AudioPlaybackController.Instance.GetCurrentTime()} | Scroll: {scrollTime}");
        }
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
    }
}
