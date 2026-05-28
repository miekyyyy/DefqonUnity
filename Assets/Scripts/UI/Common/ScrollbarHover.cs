using UnityEngine;
using UnityEngine.EventSystems;

namespace DefqonEngine.UI.Common
{

    public class ScrollbarHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool isHovering;
        private bool isPointerOverBar;
        private bool isPointerOverRect;
        [SerializeField] private RectTransform rect;

        void Update()
        {
            if (rect != null)
            {
                isPointerOverRect = RectTransformUtility.RectangleContainsScreenPoint(
                    rect,
                    Input.mousePosition
                );
            }

            if (isPointerOverRect)
            {
                isHovering = true;
                return;
            }
            if (!isPointerOverBar)
            {
                isHovering = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOverBar = true;
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOverBar = false;
            if (!isPointerOverRect)
            {
                isHovering = false;
            }
        }
    }
}
