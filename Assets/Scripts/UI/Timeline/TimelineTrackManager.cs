using DefqonEngine.Lighting.Groups;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.UI.Timeline
{
    public class TimelineTrackManager : MonoBehaviour
    {
        [Header("References")]
        public RectTransform tracksParent;     // Track Content
        public RectTransform labelsParent;     // Track Labels
        public TimelineTrack trackPrefab;
        public TimelineTrackLabel labelPrefab;

        private readonly List<TimelineTrack> tracks = new();
        private readonly Dictionary<TimelineTrack, TimelineTrackLabel> labels = new();

        public TimelineTrack AddTrack(LampGroup group)
        {
            var track = Instantiate(trackPrefab, tracksParent);
            track.lampGroup = group;

            tracks.Add(track);

            // Label
            var label = Instantiate(labelPrefab, labelsParent);
            label.track = track;
            labels[track] = label;

            RebuildLayout();
            return track;
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

        public TimelineTrack FindTrackFromGroup(LampGroup group)
        {
            foreach (var track in tracks)
                if (track.lampGroup == group)
                    return track;
            return null;
        }

        public IReadOnlyList<TimelineTrack> Tracks => tracks;
    }
}
