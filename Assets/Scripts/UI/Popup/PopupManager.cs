using DefqonEngine.Core.Project;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }
        public List<PopupCanvas> popups = new List<PopupCanvas>();
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            ProjectManager.Instance.OnProjectLoaded += ClosePopups;
            foreach (var popup in popups)
            {
                popup.OnPopupOpen += OnPopupOpen;
                popup.Close();
            }
        }

        public void OnPopupOpen(PopupCanvas popup)
        {
            foreach (var closingPopup in popups)
            {
                if (closingPopup == popup) continue;
                closingPopup.Close();
            }
        }

        public void ClosePopups()
        {
            foreach (var popup in popups)
            {
                popup.Close();
            }
        }
    }
}
