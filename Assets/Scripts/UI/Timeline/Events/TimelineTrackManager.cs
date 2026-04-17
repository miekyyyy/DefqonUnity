using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Events
{
    public class TimelineTrackManager : MonoBehaviour
    {
        public static TimelineTrackManager Instance { get; private set; }

        [Header("References")]
        public RectTransform tracksParent;     // Track Content
        public RectTransform labelsParent;     // Track Labels
        public TimelineTrack trackPrefab;
        public TimelineTrackLabel labelPrefab;

        private readonly List<TimelineTrack> tracks = new();
        private readonly Dictionary<TimelineTrack, TimelineTrackLabel> labels = new();

        public event Action<TimelineTrack> OnTrackAdded;
        public event Action<TimelineTrack> OnTrackRemoved;
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
                newTracks.Add(AddTrack());
            }
            return newTracks;
        }

        public TimelineTrack AddTrack()
        {
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

        public void RemoveTrack()
        {
            if (tracks.Count == 0) return;
            var track = tracks.Last();
            RemoveTrack(track);
        }

        public void RemoveTrack(TimelineTrack track)
        {
            if (!tracks.Contains(track)) return;

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
            RebuildLayout();
        }

        public void ClearAll()
        {
            // Destroy alle track GameObjects
            foreach (var t in tracks)
                if (t != null)
                    Destroy(t.gameObject);

            // Destroy alle labels
            foreach (var l in labels.Values)
                if (l != null)
                    Destroy(l.gameObject);

            tracks.Clear();
            labels.Clear();

            RebuildLayout();
        }

        void RebuildLayout()
        {
            float y = 0f;

            for (int i = tracks.Count - 1; i >= 0; i--)
            {
                var t = tracks[i];
                var rect = t.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(0, y);
                y += rect.sizeDelta.y;

                // Update label pos als die bestaat
                if (labels.TryGetValue(t, out var label))
                {
                    var labelRect = label.GetComponent<RectTransform>();
                    labelRect.anchoredPosition = new Vector2(labelRect.anchoredPosition.x, rect.anchoredPosition.y);
                }
            }

            tracksParent.sizeDelta = new Vector2(tracksParent.sizeDelta.x, y);
        }

        public TimelineTrack FindTrackByIndex(int index)
        {
            return tracks.FirstOrDefault(t => t.trackIndex == index);
        }


        public IReadOnlyList<TimelineTrack> Tracks => tracks;

    }
}
