using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.IO.Project;
using DefqonEngine.Sequencing.Data.Presets;
using DefqonEngine.UI.Popup;
using Newtonsoft.Json;
using SFB;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.IO.Presets
{
    public class PresetSaveManager : MonoBehaviour
    {
        private string presetName;
        
        public void SetPresetName(string name) => presetName = name;

        public void SavePreset()
        {
            //File saving
            var path = StandaloneFileBrowser.SaveFilePanel("Save Preset", "", "preset", "dfqprs");
            if (string.IsNullOrEmpty(path) || TimelineEventManager.Instance.selectedEvent == null)
                return;

            var preset = new EventPreset(
                string.IsNullOrEmpty(presetName) ? Path.GetFileNameWithoutExtension(path) : presetName,
                TimelineEventManager.Instance.selectedEvent
            );
            Save(preset, path);
            PopupManager.Instance.ClosePopups();
        }

        public void LoadPresets()
        {
            //Files loading
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Presets", "", "dfqprs", true);
            if (paths.Length == 0)
                return;

            foreach (var path in paths)
            {
                var preset = Load(path);
                if (preset != null)
                    LoadPreset(preset);
            }
        }

        public void LoadPresets(List<EventPreset> presets)
        {
            if (presets == null || presets.Count == 0)
            {
                Debug.Log("No presets to load");
                return;
            }
            foreach (var preset in presets)
            {
                LoadPreset(preset);
            }
        }

        public void LoadPreset(EventPreset preset)
        {
            if (preset == null)
            {
                Debug.Log("Preset is null, cannot load");
                return;
            }
            PresetManager.Instance.LoadPreset(preset);
        }

        public void Save(EventPreset preset, string path)
        {
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(preset, ProjectSaveManager.settings);
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
                return JsonConvert.DeserializeObject<EventPreset>(json, ProjectSaveManager.settings);
            }
            catch (JsonException ex)
            {
                Debug.Log($"Failed to load preset file: {path}\n{ex.Message}");
                return null;
            }
        }
    }
}
