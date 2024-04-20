using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementMover : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform UIElement;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool locked = true;
    [SerializeField] private Vector2 movementLock = new Vector2(0.7f, 0.7f);
    private static Vector2 _pos = Vector2.zero;

    private void Awake()
    {
        if (_pos != Vector2.zero)
        { 
            UIElement.position = _pos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        UIElement.anchoredPosition += eventData.delta / canvas.scaleFactor;

        float x = UIElement.sizeDelta.x / canvas.scaleFactor * 0.5f;
        float y = UIElement.sizeDelta.y / canvas.scaleFactor * 0.5f;

        if (locked)
        {
            float clampedX = Mathf.Clamp(UIElement.position.x, 0 + x * movementLock.x, Screen.width - x * movementLock.x);
            float clampedY = Mathf.Clamp(UIElement.position.y, 0 + y * movementLock.y, Screen.height - y);

            _pos = new Vector2(clampedX, clampedY);
        }
        else
        {
            _pos = new Vector2(x, y);
        }

        UIElement.position = _pos;
    }
}
