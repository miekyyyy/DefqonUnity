using DefqonEngine.UI.Timeline.Events;
using Newtonsoft.Json;
using SFB;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

namespace DefqonEngine.Common.Data
{
    public class PresetSaveManager : MonoBehaviour
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,  // Cruciaal voor polymorfisme
            Formatting = Formatting.Indented
        };
        public void SavePreset()
        {
            //File saving
            var path = StandaloneFileBrowser.SaveFilePanel("Save Preset", "", "preset", "json");
            if (string.IsNullOrEmpty(path))
                return;

            var preset = new EventPreset(
                Path.GetFileNameWithoutExtension(path),
                TimelineEventManager.Instance.selectedEvent
            );
            Save(preset, path);
        }

        public void LoadPresets()
        {
            //Files loading
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Presets", "", "json", true);
            if (paths.Length == 0)
                return;

            foreach (var path in paths)
            {
                var preset = Load(path);
                if (preset != null)
                    PresetManager.Instance.AddButton(preset);
            }
        }

        public void Save(EventPreset preset, string path)
        {
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(preset, settings);
            File.WriteAllText(path, json);

            Debug.Log($"Preset saved to {path}");
        }

        public EventPreset Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("No preset save found, returning null");
                return null;
            }

            try
            {

                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<EventPreset>(json, settings);
            }
            catch (JsonException ex)
            {
                Debug.Log($"Failed to load preset file: {path}\n{ex.Message}");
                return null;
            }
        }
    }
}
