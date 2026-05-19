using System;
using UnityEngine;

namespace DefqonEngine.UI.Popup
{
    public class Popup : MonoBehaviour
    {
        public event Action<Popup> OnPopupOpen;
        public event Action<Popup> OnPopupClose;
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