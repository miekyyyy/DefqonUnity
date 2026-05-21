using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.IO.Audio;
using DefqonEngine.IO.Presets;
using DefqonEngine.IO.Project;
using DefqonEngine.IO.Timeline;
using System;
using UnityEngine;

namespace DefqonEngine.Core.Project
{

    public class ProjectManager : MonoBehaviour
    {
        public static ProjectManager Instance { get; private set; }
        public DefqonProject CurrentProject { get; private set; }
        [SerializeField] private ProjectSaveManager projectSaveManager;
        [SerializeField] private PresetSaveManager presetSaveManager;
        [SerializeField] private TimelineSaveManager timelineSaveManager;
        [SerializeField] private AudioSaveManager audioSaveManager;

        private string newProjectName = "";
        private string newProjectAudioFilePath = "";

        public event Action OnProjectLoaded;
        public event Action OnProjectSaved;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void SetNewProjectName(string name)
        {
            newProjectName = name;
        }

        public void SetNewProjectAudioFilePath()
        {
            newProjectAudioFilePath = audioSaveManager.LoadAudioDialog();
        }

        public void ResetNewProjectSettings()
        {
            newProjectName = "";
            newProjectAudioFilePath = "";
        }

        public void CreateNewProject()
        {
            var newProject = new DefqonProject(newProjectName, newProjectAudioFilePath);
            bool saved = projectSaveManager.SaveProject(newProject);
            if (!saved)
            {
                Debug.Log("Project creation cancelled or save failed.");
                return;
            }
            LoadProjectData(newProject);
            OnProjectLoaded?.Invoke();
        }

        public void SaveProject()
        {
            if (CurrentProject == null)
            {
                Debug.LogError("No project to save.");
                return;
            }
            CurrentProject.events = TimelineEventManager.Instance.events;
            CurrentProject.presets = PresetManager.Instance.GetAllAddedPresets();
            CurrentProject.trackCount = TimelineTrackManager.Instance.TrackCount;
            projectSaveManager.SaveProject(CurrentProject);
            OnProjectSaved?.Invoke();
        }

        public string SaveProjectWithReturn()
        {
            if (CurrentProject == null)
            {
                Debug.LogError("No project to save.");
                return null;
            }
            CurrentProject.events = TimelineEventManager.Instance.events;
            CurrentProject.presets = PresetManager.Instance.GetAllAddedPresets();
            CurrentProject.trackCount = TimelineTrackManager.Instance.TrackCount;
            string path = projectSaveManager.SaveProjectReturnPath(CurrentProject);
            OnProjectSaved?.Invoke();
            return path;
        }

        public void LoadProject()
        {
            var project = projectSaveManager.LoadProject();
            if (project == null)
            {
                Debug.Log("Failed to load project.");
                return;
            }
            LoadProjectData(project);
            OnProjectLoaded?.Invoke();
        }

        private void LoadProjectData(DefqonProject project)
        {
            CloseProject();
            CurrentProject = project;
            presetSaveManager.LoadPresets(project.presets);
            timelineSaveManager.LoadTimeline(project.events, project.trackCount);
            audioSaveManager.LoadAudio(project.audioFilePath);
        }

        private void CloseProject()
        {
            CurrentProject = null;
            PresetManager.Instance.ClearLoadedPresets();
            TimelineEventManager.Instance.ClearEvents();
        }
    }
}