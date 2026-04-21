using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Project;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Sequencing.Data.Events;
using DefqonEngine.Sequencing.Data.Presets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.IO.Project
{
    public class ProjectSaveManager : MonoBehaviour
    {
        public static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new DefqonSerializationBinder(),
            Formatting = Formatting.Indented
        };

        private sealed class DefqonSerializationBinder : ISerializationBinder
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

        public void SaveProject(DefqonProject project)
        {
            //File saving
            var path = StandaloneFileBrowser.SaveFilePanel("Save Project", "", "project", "dfqprj");
            if (string.IsNullOrEmpty(path) || project == null)
                return;

            Save(project, path);
        }

        public DefqonProject LoadProject()
        {
            //Files loading
            var paths = StandaloneFileBrowser.OpenFilePanel("Load Project", "", "dfqprj", false);
            if (paths.Length == 0)
                return null;

            var project = Load(paths[0]);
            return project;
        }

        public void Save(DefqonProject project, string path)
        {

            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(project, settings);
            File.WriteAllText(path, json);

            Debug.Log($"Project saved to {path}");
        }

        public DefqonProject Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("No project save found, returning null");
                return null;
            }

            try
            {

                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<DefqonProject>(json, settings);
            }
            catch (JsonException ex)
            {
                Debug.Log($"Failed to load project file: {path}\n{ex.Message}");
                return null;
            }
        }
    }
}
