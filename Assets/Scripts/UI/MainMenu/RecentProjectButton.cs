using DefqonEngine.Core.Project;
using TMPro;
using UnityEngine;

namespace DefqonEngine.UI.MainMenu
{
    public class RecentProjectButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text projectNameText;
        string projectPath;


        public void Initialize(string path, string name)
        {
            projectPath = path;
            projectNameText.text = name;
        }
        
        public void OpenProject()
        {
            ProjectManager.Instance.LoadProject(projectPath);
        }
    }
}
