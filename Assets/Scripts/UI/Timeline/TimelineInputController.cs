using UnityEngine;

namespace DefqonEngine.UI.Timeline
{
    public class TimelineInputController : MonoBehaviour
    {

        void Update()
        {
            HandleZoom();
            HandlePan();
        }

        void HandleZoom()
        {
            if (!Input.GetKey(KeyCode.LeftControl)) return;
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            Vector2 localMouse;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                TimelineView.Instance.panel,
                Input.mousePosition,
                null,
                out localMouse
            );

            TimelineView.Instance.Zoom(scroll > 0 ? 1.1f : 0.9f, localMouse.x);
        }

        void HandlePan()
        {
            if (Input.GetMouseButton(2))
            {
                float deltaX = Input.GetAxis("Mouse X") * 20f;
                TimelineView.Instance.PanPixels(deltaX);
            }
        }
    }
}
