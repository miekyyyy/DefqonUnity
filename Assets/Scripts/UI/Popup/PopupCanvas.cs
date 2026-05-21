using System;
using UnityEngine;

namespace DefqonEngine.UI.Popup
{
    public class PopupCanvas : MonoBehaviour
    {
        public event Action<PopupCanvas> OnPopupOpen;
        public event Action<PopupCanvas> OnPopupClose;
        public void Open()
        {
            gameObject.SetActive(true);
            OnPopupOpen?.Invoke(this);
        }
        public void Close()
        {
            gameObject.SetActive(false);
            OnPopupClose?.Invoke(this);
        }
    }
}