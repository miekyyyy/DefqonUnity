using DefqonEngine.Common;
using DefqonEngine.Common.Data;
using DefqonEngine.UI.Timeline.Events;
using Newtonsoft.Json;
using SFB;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Common
{
    public class TimelineSaveManager : MonoBehaviour
    {

        public void SaveCurrentManager()
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Save Timeline", "", "events", "json");
            if (string.IsNullOrEmpty(path))
                return;
            TimelineEventManager manager = TimelineEventManager.Instance;
            Save(manager.events, path);
        }
        public void LoadToCurrentManager()
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Timeline", "", "json", false);
            if (paths.Length == 0)
                return;

            TimelineEventManager manager = TimelineEventManager.Instance;
            List<TimelineEvent> events = Load(paths[0]);
            int maxTrackIndex = -1;
            foreach (var ev in events)
            {
                if (ev.trackIndex > maxTrackIndex)
                    maxTrackIndex = ev.trackIndex;
            }
            if(TimelineTrackManager.Instance.TrackCount <= maxTrackIndex)
                TimelineTrackManager.Instance.AddTracks(maxTrackIndex + 1 - TimelineTrackManager.Instance.TrackCount);
            manager.SetEvents(events);
        }

        public void Save(List<TimelineEvent> events, string path)
        {
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(events, PresetSaveManager.settings);
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
                return JsonConvert.DeserializeObject<List<TimelineEvent>>(json, PresetSaveManager.settings)
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
