using UnityEngine;
using UnityEngine.EventSystems;

public class UIElementMover : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform UIElement;
    [SerializeField] private Canvas canvas;

    public void OnDrag(PointerEventData eventData)
    {
        UIElement.anchoredPosition += eventData.delta / canvas.scaleFactor;

        float x = UIElement.sizeDelta.x / canvas.scaleFactor * 0.5f;
        float y = UIElement.sizeDelta.y / canvas.scaleFactor * 0.5f;

        UIElement.position = new Vector3(Mathf.Clamp(UIElement.position.x, 0 + x, Screen.width - x),
            Mathf.Clamp(UIElement.position.y, 0 + y, Screen.height - y), UIElement.position.z);
    }
}
