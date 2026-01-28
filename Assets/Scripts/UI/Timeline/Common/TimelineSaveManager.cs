using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DefqonEngine.UI.Timeline.Development.Events;

namespace DefqonEngine.UI.Timeline.Common
{
    public class TimelineSaveManager : MonoBehaviour
    {
        [SerializeField] string directoryName = "TimelineSaves";
        string SavePath => Path.Combine(Application.persistentDataPath, directoryName, "events.json");

        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,  // Cruciaal voor polymorfisme
            Formatting = Formatting.Indented
        };

        public void SaveCurrentManager()
        {
            TimelineEventManager manager = TimelineEventManager.Instance;
            Save(manager.events);
        }
        public void LoadToCurrentManager()
        {
            TimelineEventManager manager = TimelineEventManager.Instance;
            List<TimelineEvent> events = Load();
            manager.SetEvents(events);
        }

        public void Save(List<TimelineEvent> events)
        {
            string json = JsonConvert.SerializeObject(events, settings);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Timeline saved to {SavePath}");
        }

        public List<TimelineEvent> Load()
        {
            if (!File.Exists(SavePath))
                return new List<TimelineEvent>();

            string json = File.ReadAllText(SavePath);
            List<TimelineEvent> events = JsonConvert.DeserializeObject<List<TimelineEvent>>(json, settings);
            Debug.Log($"Timeline loaded from {SavePath}, {events.Count} events");
            return events;
        }
    }
}
