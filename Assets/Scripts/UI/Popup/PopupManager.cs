using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }
        public List<Popup> popups = new List<Popup>();
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (popups == null)
            {
                return;
            }

            foreach (var popup in popups)
            {
                popup.Close();
            }
        }

        public void OnPopupOpen(Popup popup)
        {
            if (popups == null)
            {
                return;
            }

            foreach (var closingPopup in popups)
            {
                if(closingPopup == popup) continue;
                closingPopup.Close();
            }
        }

        public void ClosePopups()
        {
            if (popups == null)
            {
                return;
            }

            foreach (var popup in popups)
            {
                popup.Close();
            }
        }
    }
}
