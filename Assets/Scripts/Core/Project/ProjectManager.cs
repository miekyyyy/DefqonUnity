using DefqonEngine.Core.Presets;
using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.IO.Audio;
using DefqonEngine.IO.Presets;
using DefqonEngine.IO.Project;
using DefqonEngine.IO.Timeline;
using DefqonEngine.UI.Popup;
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
        }

        public void SaveProject()
        {
            if (CurrentProject == null)
            {
                Debug.LogError("No project to save.");
                return;
            }
            CurrentProject.events = TimelineEventManager.Instance.events;
            CurrentProject.presets = PresetManager.Instance.GetAllPresets();
            CurrentProject.trackCount = TimelineTrackManager.Instance.TrackCount;
            projectSaveManager.SaveProject(CurrentProject);
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
            PopupManager.Instance.ClosePopups();
        }

        private void LoadProjectData(DefqonProject project)
        {
            CurrentProject = project;
            presetSaveManager.LoadPresets(project.presets);
            timelineSaveManager.LoadTimeline(project.events, project.trackCount);
            audioSaveManager.LoadAudio(project.audioFilePath);
        }
    }
}