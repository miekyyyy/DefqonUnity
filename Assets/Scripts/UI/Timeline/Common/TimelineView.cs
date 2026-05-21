using DefqonEngine.Core.Project;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.Sequencing.Audio;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Timeline.Control;
using DefqonEngine.UI.Timeline.Waveform;
using System;
using UnityEngine;
using UnityEngine.UI;
using static DefqonEngine.UI.Timeline.Control.TimelineInputController;

namespace DefqonEngine.UI.Timeline.Common
{
    public class TimelineView : MonoBehaviour
    {

        public static TimelineView Instance { get; private set; }
        [Header("View")]
        public RectTransform eventsPanel;
        public RectTransform viewport;

        [Header("Scrollbars")]
        [SerializeField] private Scrollbar horizontalScrollbar;
        [SerializeField] private Scrollbar verticalScrollbar;

        [Header("Zoom")]
        private float pixelsPerSecond = 100f;

        [Header("Scroll")]
        public float scrollTime = 0f;
        private float verticalScroll;

        [Header("Snapping")]
        public float gridSize = 0.25f; // 1/4 seconde grid
        public float eventSnapRange = 0.1f;
        public SnappingMode snappingMode = SnappingMode.None;


        public Action OnViewChanged;
        public float Width => eventsPanel.rect.width;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            TimelineInputController.Instance.OnZoom += Zoom;
            TimelineInputController.Instance.OnPan += PanPixels;
            TimelineTrackManager.Instance.OnRebuildLayout += RefreshScrollbars;
            WaveformDrawer.Instance.OnWaveformLoaded += RefreshScrollbars;

            if (horizontalScrollbar != null)
                horizontalScrollbar.onValueChanged.AddListener(OnHorizontalScrollbarChanged);

            if (verticalScrollbar != null)
                verticalScrollbar.onValueChanged.AddListener(OnVerticalScrollbarChanged);

            RefreshScrollbars();
        }
        private void OnDestroy()
        {
            TimelineInputController.Instance.OnZoom -= Zoom;
            TimelineInputController.Instance.OnPan -= PanPixels;

            if (horizontalScrollbar != null)
                horizontalScrollbar.onValueChanged.RemoveListener(OnHorizontalScrollbarChanged);

            if (verticalScrollbar != null)
                verticalScrollbar.onValueChanged.RemoveListener(OnVerticalScrollbarChanged);
        }

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
        public float TimeToX(float time)
        {
            return (time - scrollTime) * pixelsPerSecond;
        }

        // X position in pixels to time in seconds, relative to the left edge of the timeline
        public float XToTime(float x)
        {
            return (x / pixelsPerSecond) + scrollTime;
        }

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
            if (!AudioPlaybackController.Instance || !AudioPlaybackController.Instance.IsPlaying())
                return;
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
            if (AudioPlaybackController.Instance == null ||
                AudioPlaybackController.Instance.GetAudioClip() == null)
            {
                newScrollTime = Mathf.Max(0f, newScrollTime);
            }
            else
            {
                float maxScroll = Mathf.Max(
                    0f,
                    AudioPlaybackController.Instance.GetAudioClip().length
                    - (Width / pixelsPerSecond));

                newScrollTime = Mathf.Clamp(
                    newScrollTime,
                    0f,
                    maxScroll);
            }

            scrollTime = newScrollTime;

            RefreshScrollbars();
            OnViewChanged?.Invoke();
        }

        private void OnHorizontalScrollbarChanged(float value)
        {
            var clip = AudioPlaybackController.Instance?.GetAudioClip();
            if (clip == null) return;

            float maxScroll =
                Mathf.Max(0f, clip.length - VisibleDuration);

            SetScrollTime(value * maxScroll);
        }
        private void RefreshHorizontalScrollbar()
        {
            if (horizontalScrollbar == null)
                return;

            var clip =
                AudioPlaybackController.Instance?.GetAudioClip();

            if (clip == null)
            {
                horizontalScrollbar.size = 1f;
                horizontalScrollbar.value = 0f;
                return;
            }

            float clipLength = clip.length;
            float visible = VisibleDuration;

            float maxScroll =
                Mathf.Max(0f, clipLength - visible);

            horizontalScrollbar.size =
                Mathf.Clamp01(visible / clipLength);

            horizontalScrollbar.SetValueWithoutNotify(
                maxScroll <= 0f
                    ? 0f
                    : scrollTime / maxScroll);
        }
        public void RefreshScrollbars()
        {
            RefreshHorizontalScrollbar();
            RefreshVerticalScrollbar();
            ApplyVerticalScroll();
        }
        private void ApplyVerticalScroll()
        {
            var manager = TimelineTrackManager.Instance;
            if (manager == null) return;

            RectTransform tracksPanel = manager.tracksParent;

            float contentHeight = tracksPanel.sizeDelta.y;
            float viewportHeight = viewport.rect.height;

            float maxScroll =
                Mathf.Max(0f, contentHeight - viewportHeight);

            float y = verticalScroll * maxScroll;

            tracksPanel.anchoredPosition =
                new Vector2(tracksPanel.anchoredPosition.x, y);
            eventsPanel.anchoredPosition = tracksPanel.anchoredPosition;

            // Move labels too
            manager.labelsParent.anchoredPosition =
                new Vector2(
                    manager.labelsParent.anchoredPosition.x,
                    y);
        }
        
        private void OnVerticalScrollbarChanged(float value)
        {
            verticalScroll = value;
            ApplyVerticalScroll();
        }
        private void RefreshVerticalScrollbar()
        {
            if (verticalScrollbar == null)
                return;

            var manager = TimelineTrackManager.Instance;
            if (manager == null)
                return;

            float contentHeight =
                manager.tracksParent.sizeDelta.y;

            float viewportHeight =
                eventsPanel.rect.height;

            float maxScroll =
                Mathf.Max(0f, contentHeight - viewportHeight);

            verticalScrollbar.size =
                contentHeight <= 0f
                    ? 1f
                    : Mathf.Clamp01(viewportHeight / contentHeight);

            verticalScrollbar.SetValueWithoutNotify(
                maxScroll <= 0f
                    ? 0f
                    : verticalScroll);
        }


        public void PanPixels(float deltaX)
        {
            float deltaTime = deltaX / pixelsPerSecond;
            SetScrollTime(scrollTime - deltaTime);
        }

        public void Zoom(ZoomData zoomData)
        {
            if (pixelsPerSecond <= 0f) return;

            float timeUnderMouse = XToTime(zoomData.zoomCenterX);

            // Nieuwe pixelsPerSecond
            float newPPS = pixelsPerSecond * zoomData.zoomFactor;

            // Clamp: niet verder uitzoomen dan de clip
            if (AudioPlaybackController.Instance != null && AudioPlaybackController.Instance.GetAudioClip() != null)
            {
                float minPPS = Width / AudioPlaybackController.Instance.GetAudioClip().length;
                newPPS = Mathf.Max(newPPS, minPPS);
            }

            pixelsPerSecond = Mathf.Clamp(newPPS, 20f, 600f); // Max zoom in

            // Pas scrollTime aan zodat de tijd onder de muis blijft
            float newScrollTime = timeUnderMouse - (zoomData.zoomCenterX / pixelsPerSecond);
            SetScrollTime(newScrollTime);
            RefreshScrollbars();
        }

        public float SnapTime(float time, SnapContext context, TimelineEvent ignore = null, float duration = 0f)
        {
            switch (snappingMode)
            {
                case SnappingMode.Time:
                    return SnapToGrid(time);
                case SnappingMode.Events:
                    return SnapToEvents(time, duration, ignore, context);
                case SnappingMode.None:
                default:
                    return time;
            }
        }

        private float SnapToGrid(float time)
        {
            return Mathf.Round(time / gridSize) * gridSize;
        }

        private float SnapToEvents(float time, float duration, TimelineEvent ignore, SnapContext context)
        {
            float best = time;
            float bestDist = eventSnapRange;

            foreach (var e in TimelineEventManager.Instance.events)
            {
                if (e == ignore) continue;

                float start = e.time;
                float end = e.time + e.duration;

                if (context == SnapContext.ResizeStart)
                {
                    // snap start handle to both starts and ends
                    float dStart = Mathf.Abs(start - time);
                    if (dStart < bestDist)
                    {
                        bestDist = dStart;
                        best = start;
                    }

                    float dEnd = Mathf.Abs(end - time);
                    if (dEnd < bestDist)
                    {
                        bestDist = dEnd;
                        best = end;
                    }
                }
                else if (context == SnapContext.ResizeEnd)
                {
                    // snap end handle to both starts and ends
                    float dStart = Mathf.Abs(start - time);
                    if (dStart < bestDist)
                    {
                        bestDist = dStart;
                        best = start;
                    }

                    float dEnd = Mathf.Abs(end - time);
                    if (dEnd < bestDist)
                    {
                        bestDist = dEnd;
                        best = end;
                    }
                }
                else // Move
                {
                    // snap to starts and ends, from both start and the end of current event.
                    float moveStartToStart = Mathf.Abs(start - time);
                    float moveStartToEnd = Mathf.Abs(end - time);

                    float moveEndToStart = Mathf.Abs((start - duration) - time);
                    float moveEndToEnd = Mathf.Abs((end - duration) - time);

                    if (moveStartToStart < bestDist)
                    {
                        bestDist = moveStartToStart;
                        best = start;
                    }

                    if (moveStartToEnd < bestDist)
                    {
                        bestDist = moveStartToEnd;
                        best = end;
                    }

                    if (moveEndToStart < bestDist)
                    {
                        bestDist = moveEndToStart;
                        best = start - duration;
                    }

                    if (moveEndToEnd < bestDist)
                    {
                        bestDist = moveEndToEnd;
                        best = end - duration;
                    }
                }

                // Snap to playhead
                float playheadTime = AudioPlaybackController.Instance != null ? AudioPlaybackController.Instance.GetCurrentTime() : 0f;

                float dPlayheadStart = Mathf.Abs(playheadTime - time);
                if (dPlayheadStart < bestDist)
                {
                    bestDist = dPlayheadStart;
                    best = playheadTime;
                }

                // also allow “end-aligned feel” for duration-based operations
                float dPlayheadEnd = Mathf.Abs(playheadTime - (time + duration));
                if (dPlayheadEnd < bestDist)
                {
                    bestDist = dPlayheadEnd;
                    best = playheadTime - duration;
                }
            }

            return best;
        }

        public void UpdateSnappingMode()
        {
            switch (snappingMode)
            {
                case SnappingMode.None:
                    snappingMode = SnappingMode.Time;
                    break;
                case SnappingMode.Time:
                    snappingMode = SnappingMode.Events;
                    break;
                case SnappingMode.Events:
                    snappingMode = SnappingMode.None;
                    break;
            }
        }
    }
}
