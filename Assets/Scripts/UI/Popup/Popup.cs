using UnityEngine;

namespace DefqonEngine.UI.Popup {
    public class Popup : MonoBehaviour
    {
        public void Open() {
            gameObject.SetActive(true);
            PopupManager.Instance.OnPopupOpen(this);
        }
        public void Close() {
            gameObject.SetActive(false);
        }
    }
}