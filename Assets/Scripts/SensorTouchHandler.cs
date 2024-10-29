using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SensorTouchHandler : MonoBehaviour
{
    public GraphicRaycaster raycaster;  // CanvasにアタッチされているGraphicRaycaster
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    public void ProcessSensorTouch(SensorTouch sensorTouch)
    {
        // SensorTouchの位置をスクリーン座標に変換
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(sensorTouch.position);

        // PointerEventDataを初期化
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = screenPosition
        };

        // グラフィックレイキャストを使用して、指定の位置にあるUI要素を取得
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // UI要素が存在する場合、その最初の要素に対してクリックイベントを発生
        foreach (var result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                // ボタンのクリックイベントを擬似的に呼び出し
                button.onClick.Invoke();
                Debug.Log($"Button {button.name} clicked by SensorTouch.");
                break;  // 最初のボタンのみ処理
            }
        }
    }
}
