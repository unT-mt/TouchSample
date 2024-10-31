using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up");
    }
}
