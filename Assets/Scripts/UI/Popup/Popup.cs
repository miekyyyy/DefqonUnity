using UnityEngine;

namespace DefqonEngine.UI.Popup {
    public class Popup : MonoBehaviour
    {
        public void Open() {
            gameObject.SetActive(true);

            var popupManager = PopupManager.Instance;
            if (popupManager == null) {
                Debug.LogWarning("Popup.Open() called but PopupManager.Instance is null.", this);
                return;
            }

            popupManager.OnPopupOpen(this);
        }
        public void Close() {
            gameObject.SetActive(false);
        }
    }
}