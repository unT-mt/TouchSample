using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Custom Button Events")]
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;
    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

    // PointerDown時の処理（OnClickを発火させる）
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        // 通常のOnClickイベントも含め、onPointerDownイベントを発火
        onPointerDown?.Invoke();
        onClick?.Invoke();  // これでPointerDown時にOnClick相当の処理を呼び出し
    }

    // PointerUp時の処理
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onPointerUp?.Invoke();
    }

    // PointerEnter時の処理
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onPointerEnter?.Invoke();
    }

    // PointerExit時の処理
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onPointerExit?.Invoke();
    }
}
