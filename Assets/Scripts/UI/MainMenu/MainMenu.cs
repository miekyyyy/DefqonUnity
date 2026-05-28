using DefqonEngine.Core.Project;
using DefqonEngine.IO.Project;
using DefqonEngine.Sequencing.Data.Presets;
using DefqonEngine.UI.Popup;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefqonEngine.UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu Instance { get; private set; }
        [SerializeField] private ProjectSaveManager projectSaveManager;
        [SerializeField] private Transform projectContainer;
        [SerializeField] private RecentProjectButton recentProjectButtonPrefab;

        [SerializeField] PopupCanvas mainMenuPopup;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private void Start()
        {
            ProjectManager.Instance.OnProjectLoaded += CloseMainMenu;

            var projects = projectSaveManager.LoadRecentProjects();

            Debug.Log($"Loaded {projects.Count} recent projects");
            Debug.Log($"Recent project count: {projectSaveManager.recentProjectPaths.Count}");
            for (int i = 0; i < projectSaveManager.recentProjectPaths.Count; i++)
            {
                string projectPath = projectSaveManager.recentProjectPaths[i];
                var button = Instantiate(recentProjectButtonPrefab, projectContainer);
                button.Initialize(projectPath, projects[i].projectName);
                Debug.Log($"Added recent project button for {projects[i].projectName} at path {projectPath}");
            }
        }

        private void CloseMainMenu()
        {
            mainMenuPopup.Close();
        }
    }
}
