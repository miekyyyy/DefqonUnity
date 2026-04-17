using DefqonEngine.Lighting.Data;
using DefqonEngine.UI.Timeline.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.Common.Data
{
    public class PresetSaveManager : MonoBehaviour
    {
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
