using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.IO.Project;
using DefqonEngine.Sequencing.Data.Events;
using Newtonsoft.Json;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.IO.Timeline
{
    public class TimelineSaveManager : MonoBehaviour
    {

        public void SaveCurrentManager()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Save Timeline", "", "events", "dfqtml");
            if (string.IsNullOrEmpty(path))
                return;
            TimelineEventManager manager = TimelineEventManager.Instance;
            Save(manager.events, path);
        }
        public void LoadToCurrentManager()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Timeline", "", "dfqtml", false);
            if (paths.Length == 0)
                return;

            if (string.IsNullOrEmpty(paths[0]))
                return;
            LoadTimeline(Load(paths[0]));
        }

        public void LoadTimeline(List<TimelineEvent> events, int trackCount = 0)
        {
            int maxTrackIndex = -1;
            foreach (var ev in events)
            {
                if (ev.trackIndex > maxTrackIndex)
                    maxTrackIndex = ev.trackIndex;
            }

            int requiredTracks = Math.Max(trackCount, maxTrackIndex + 1);
            int currentTracks = TimelineTrackManager.Instance.TrackCount;

            if (currentTracks < requiredTracks)
            {
                TimelineTrackManager.Instance.AddTracks(requiredTracks - currentTracks);
            }
            TimelineEventManager.Instance.SetEvents(events);
        }

        public void Save(List<TimelineEvent> events, string path)
        {
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(events, ProjectSaveManager.settings);
            File.WriteAllText(path, json);

            Debug.Log($"Timeline saved to {path}");
        }

        public List<TimelineEvent> Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("No timeline save found, returning empty list");
                return new List<TimelineEvent>();
            }

            try
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<TimelineEvent>>(json, ProjectSaveManager.settings)
                       ?? new List<TimelineEvent>();
            }
            catch (JsonException ex)
            {
                Debug.Log($"Failed to load timeline file: {path}\n{ex.Message}");
                return new List<TimelineEvent>();
            }
        }
    }
}
