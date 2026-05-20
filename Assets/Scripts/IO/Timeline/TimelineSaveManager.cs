using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
<<<<<<< HEAD
using DefqonEngine.IO.Presets;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.UI.Timeline.Control;
using DefqonEngine.UI.Timeline.Events;
using Newtonsoft.Json;
using SFB;
=======
using DefqonEngine.IO.Project;
using DefqonEngine.Sequencing.Data.Events;
using Newtonsoft.Json;
using SFB;
using System;
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.IO.Timeline
{
    public class TimelineSaveManager : MonoBehaviour
    {

        public void SaveCurrentManager()
        {
<<<<<<< HEAD
            var path = StandaloneFileBrowser.SaveFilePanel("Save Timeline", "", "events", "json");
=======
            var path = StandaloneFileBrowser.SaveFilePanel("Save Timeline", "", "events", "dfqtml");
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
            if (string.IsNullOrEmpty(path))
                return;
            TimelineEventManager manager = TimelineEventManager.Instance;
            Save(manager.events, path);
        }
        public void LoadToCurrentManager()
        {
<<<<<<< HEAD
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Timeline", "", "json", false);
            if (paths.Length == 0)
                return;

            TimelineEventManager manager = TimelineEventManager.Instance;
            List<TimelineEvent> events = Load(paths[0]);
=======
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Timeline", "", "dfqtml", false);
            if (paths.Length == 0)
                return;

            if (string.IsNullOrEmpty(paths[0]))
                return;
            LoadTimeline(Load(paths[0]));
        }

        public void LoadTimeline(List<TimelineEvent> events, int trackCount = 0)
        {
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
            int maxTrackIndex = -1;
            foreach (var ev in events)
            {
                if (ev.trackIndex > maxTrackIndex)
                    maxTrackIndex = ev.trackIndex;
            }
<<<<<<< HEAD
            if (TimelineTrackManager.Instance.TrackCount <= maxTrackIndex)
                TimelineTrackManager.Instance.AddTracks(maxTrackIndex + 1 - TimelineTrackManager.Instance.TrackCount);
            manager.SetEvents(events);
=======

            int requiredTracks = Math.Max(trackCount, maxTrackIndex + 1);
            int currentTracks = TimelineTrackManager.Instance.TrackCount;

            if (currentTracks < requiredTracks)
            {
                TimelineTrackManager.Instance.AddTracks(requiredTracks - currentTracks);
            }
            TimelineEventManager.Instance.SetEvents(events);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
        }

        public void Save(List<TimelineEvent> events, string path)
        {
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

<<<<<<< HEAD
            string json = JsonConvert.SerializeObject(events, PresetSaveManager.settings);
=======
            string json = JsonConvert.SerializeObject(events, ProjectSaveManager.settings);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
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
<<<<<<< HEAD
                return JsonConvert.DeserializeObject<List<TimelineEvent>>(json, PresetSaveManager.settings)
=======
                return JsonConvert.DeserializeObject<List<TimelineEvent>>(json, ProjectSaveManager.settings)
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
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
