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
        [SerializeField] private int maxRecentProjects = 7;
        public string lastProjectPath = "";
        public List<string> recentProjectPaths = new List<string>();


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

                throw new JsonSerializationException($"Type '{typeName}' is not allowed for Defqon deserialization.");
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                if (!AllowedTypes.ContainsValue(serializedType))
                {
                    throw new JsonSerializationException($"Type '{serializedType.FullName}' is not allowed for Defqon serialization.");
                }

                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }

        public string SaveProjectWithDialog(DefqonProject project)
        {
            //File saving
            var path = StandaloneFileBrowser.SaveFilePanel("Save Project", "", "project", "dfqprj");
            if (string.IsNullOrEmpty(path) || project == null)
                return null;

            Save(project, path);
            lastProjectPath = path;
            return path;
        }

        public string SaveProjectToLastPath(DefqonProject project)
        {
            if (string.IsNullOrEmpty(lastProjectPath) || project == null)
                return null;
            Save(project, lastProjectPath);
            return lastProjectPath;
        }

        public string SaveProjectTryLastPath(DefqonProject project)
        {
            if (!string.IsNullOrEmpty(lastProjectPath))
            {
                DefqonProject oldProject = Load(lastProjectPath); // Check if the last path is valid and can be loaded
                if (oldProject != null)
                {
                    SaveProjectToLastPath(project);
                }
            }
            // If saving to last path failed, fall back to save dialog
            return SaveProjectWithDialog(project);
        }

        public DefqonProject LoadProject(string path = null)
        {
            //Files loading
            var paths = string.IsNullOrEmpty(path) ? StandaloneFileBrowser.OpenFilePanel("Load Project", "", "dfqprj", false) : new string[] { path };
            if (paths.Length == 0)
                return null;

            var project = Load(paths[0]);
            if (project != null)
            {
                lastProjectPath = paths[0];
            }
            Debug.Log($"Project loaded from {paths[0]}");
            AddToRecentProjects(project, paths[0]);

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


        #region Recent Projects
        public void AddToRecentProjects(DefqonProject project, string path)
        {
            if (project == null || string.IsNullOrEmpty(path))
                return;

            Debug.Log("Adding project to recent list: " + path);
            // Remove if project already exists to avoid duplicates
            recentProjectPaths.RemoveAll(p => p == path);

            // Add to the front of the list
            recentProjectPaths.Insert(0, path);

            // Limit to maxRecentProjects
            if (recentProjectPaths.Count > maxRecentProjects)
                recentProjectPaths.RemoveAt(recentProjectPaths.Count - 1);
            SaveRecentProjects();

        }

        public List<DefqonProject> LoadRecentProjects()
        {
            recentProjectPaths = LoadRecentList(Path.Combine(Application.persistentDataPath, "recent_projects.json"));
            Debug.Log($"Loaded recent project paths: {recentProjectPaths?.Count ?? 0}");
            if (recentProjectPaths == null || recentProjectPaths.Count == 0)
                return new List<DefqonProject>();

            List<string> pathsToRemove = new List<string>();
            var projects = new List<DefqonProject>();
            foreach (var path in recentProjectPaths)
            {
                var project = Load(path);
                if (project == null)
                {
                    pathsToRemove.Add(path);
                    continue;
                }
                projects.Add(project);
            }
            RemoveRecentProjects(pathsToRemove);
            return projects;
        }

        void RemoveRecentProjects(List<string> paths)
        {
            foreach (var path in paths)
            {
                recentProjectPaths.RemoveAll(p => p == path);
            }
        }

        public void SaveRecentProjects()
        {
            SaveRecentList(Path.Combine(Application.persistentDataPath, "recent_projects.json"));
        }

        public void SaveRecentList(string path)
        {
            Debug.Log($"Saving recent project paths: {recentProjectPaths.Count} to {path}");
            // Ensure directory exists
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonConvert.SerializeObject(recentProjectPaths);
            File.WriteAllText(path, json);

            Debug.Log($"Project list saved to {path}");
        }

        public List<string> LoadRecentList(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("No project list save found, returning null");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                Debug.Log($"Project list loaded from {path}");
                return JsonConvert.DeserializeObject<List<string>>(json);
            }
            catch (JsonException ex)
            {
                Debug.Log($"Failed to load project list file: {path}\n{ex.Message}");
                return null;
            }
        }
        #endregion
    }
}
