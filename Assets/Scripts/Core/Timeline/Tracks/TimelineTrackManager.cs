using DefqonEngine.Core.Timeline.History;
using DefqonEngine.UI.Timeline.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefqonEngine.Core.Timeline.Tracks
{
    public class TimelineTrackManager : MonoBehaviour
    {
        public static TimelineTrackManager Instance { get; private set; }

        [Header("References")]
        public RectTransform tracksParent;     // Track Content
        public RectTransform labelsParent;     // Track Labels
        public TimelineTrack trackPrefab;
        public TimelineTrackLabel labelPrefab;
        [Header("Settings")]
        [SerializeField] float distBetweenTracks = 5f;

        private readonly List<TimelineTrack> tracks = new();
        private readonly Dictionary<TimelineTrack, TimelineTrackLabel> labels = new();

        public event Action<TimelineTrack> OnTrackAdded;
        public event Action<TimelineTrack> OnTrackRemoved;
        public event Action OnRebuildLayout;
        void Awake()
        {
            Instance = this;
        }

        public int TrackCount => tracks.Count;

        public List<TimelineTrack> AddTracks(int count)
        {
            List<TimelineTrack> newTracks = new List<TimelineTrack>();
            for (int i = 0; i < count; i++)
            {
                newTracks.Add(AddTrack(false));
            }
            return newTracks;
        }

        public TimelineTrack AddTrack(bool saveState = true)
        {
            if (saveState)
            {
                TimelineHistory.Instance.SaveState("Adding 1 Track");
            }

            var track = Instantiate(trackPrefab, tracksParent);
            track.trackIndex = tracks.Count;

            tracks.Add(track);

            // Label
            var label = Instantiate(labelPrefab, labelsParent);
            label.track = track;
            labels[track] = label;

            RebuildLayout();
            OnTrackAdded?.Invoke(track);
            return track;
        }

        public void RemoveTrack(bool rebuild = true)
        {
            TimelineHistory.Instance.SaveState("Removing Last Track");

            if (tracks.Count == 0) return;
            var track = tracks.Last();
            RemoveTrack(track, rebuild);
        }

        void RemoveTrack(TimelineTrack track, bool rebuild = true)
        {
            if (!tracks.Contains(track))
                return;

            tracks.Remove(track);

            Destroy(track.gameObject);

            // Destroy label
            if (labels.TryGetValue(track, out var label))
            {
                labels.Remove(track);

                if (label != null)
                    Destroy(label.gameObject);
            }

            OnTrackRemoved?.Invoke(track);

            if (!rebuild) return;
            RebuildLayout();
        }

        public void ClearAll()
        {
            List<TimelineTrack> tracksToRemove = new List<TimelineTrack>(tracks);
            foreach (var t in tracksToRemove)
                RemoveTrack(t, false);
            RebuildLayout();
        }
        void RebuildLayout()
        {
            float y = 0f;

            for (int i = 0; i < tracks.Count; i++)
            {
                var t = tracks[i];
                var rect = t.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(0, -y);
                y += rect.sizeDelta.y + distBetweenTracks;

                if (labels.TryGetValue(t, out var label))
                {
                    var labelRect = label.GetComponent<RectTransform>();
                    labelRect.anchoredPosition = new Vector2(
                        labelRect.anchoredPosition.x,
                        rect.anchoredPosition.y);
                }
            }

            tracksParent.sizeDelta = new Vector2(tracksParent.sizeDelta.x, y);
            OnRebuildLayout?.Invoke();
        }

        public TimelineTrack FindTrackByIndex(int index)
        {
            return tracks.FirstOrDefault(t => t.trackIndex == index);
        }


        public IReadOnlyList<TimelineTrack> Tracks => tracks;

    }
}
