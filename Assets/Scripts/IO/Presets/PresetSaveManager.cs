using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Timeline.Events;
<<<<<<< HEAD
=======
using DefqonEngine.IO.Project;
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Sequencing.Data.Presets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.IO.Presets
{
    public class PresetSaveManager : MonoBehaviour
    {
<<<<<<< HEAD
        public static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new PresetSerializationBinder(),
            Formatting = Formatting.Indented
        };

        private sealed class PresetSerializationBinder : ISerializationBinder
        {
            private static readonly Dictionary<string, Type> AllowedTypes = new Dictionary<string, Type>
            {
                [typeof(EventPreset).FullName] = typeof(EventPreset),
                [typeof(TimelineEvent).FullName] = typeof(TimelineEvent),
                [typeof(LightEvent).FullName] = typeof(LightEvent),
                [typeof(SmokeEvent).FullName] = typeof(SmokeEvent)
            };

            public Type BindToType(string assemblyName, string typeName)
            {
                if (AllowedTypes.TryGetValue(typeName, out var type))
                {
                    return type;
                }

                throw new JsonSerializationException($"Type '{typeName}' is not allowed for preset deserialization.");
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                if (!AllowedTypes.ContainsValue(serializedType))
                {
                    throw new JsonSerializationException($"Type '{serializedType.FullName}' is not allowed for preset serialization.");
                }

                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }
        public void SavePreset()
        {
            //File saving
            var path = StandaloneFileBrowser.SaveFilePanel("Save Preset", "", "preset", "json");
=======
        public void SavePreset()
        {
            //File saving
            var path = StandaloneFileBrowser.SaveFilePanel("Save Preset", "", "preset", "dfqprs");
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
            if (string.IsNullOrEmpty(path) || TimelineEventManager.Instance.selectedEvent == null)
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
<<<<<<< HEAD
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Presets", "", "json", true);
=======
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Presets", "", "dfqprs", true);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
            if (paths.Length == 0)
                return;

            foreach (var path in paths)
            {
                var preset = Load(path);
                if (preset != null)
<<<<<<< HEAD
                    PresetManager.Instance.AddButton(preset);
            }
        }

=======
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

>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
        public void Save(EventPreset preset, string path)
        {
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

<<<<<<< HEAD
            string json = JsonConvert.SerializeObject(preset, settings);
=======
            string json = JsonConvert.SerializeObject(preset, ProjectSaveManager.settings);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
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
<<<<<<< HEAD
                return JsonConvert.DeserializeObject<EventPreset>(json, settings);
=======
                return JsonConvert.DeserializeObject<EventPreset>(json, ProjectSaveManager.settings);
>>>>>>> d921fedd28b702c5664981b56c3fd1bef1188ef1
            }
            catch (JsonException ex)
            {
                Debug.Log($"Failed to load preset file: {path}\n{ex.Message}");
                return null;
            }
        }
    }
}
