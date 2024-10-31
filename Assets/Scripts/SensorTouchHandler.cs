using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SensorTouchHandler : MonoBehaviour
{
    public GraphicRaycaster raycaster;  // CanvasにアタッチされているGraphicRaycaster
    private EventSystem eventSystem;
    
    void Start()
    {
        eventSystem = EventSystem.current;

        // Camera.mainがnullでないことを確認
        if (Camera.main == null)
        {
            Debug.LogError("Main camera is not found. Ensure the camera has the 'MainCamera' tag.");
        }
    }

    public void ProcessSensorTouch(SensorTouch sensorTouch)
    {
        // Camera.mainの確認と位置変換のデバッグ
        if (Camera.main != null)
        {
            // SensorTouchの位置をスクリーン座標に変換
            //Vector2 screenPosition = Camera.main.WorldToScreenPoint(sensorTouch.position);
            //Debug.Log($"Screen Position: {screenPosition}");

            // PointerEventDataを初期化
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = sensorTouch.position
            };

            // グラフィックレイキャストを使用して、指定の位置にあるUI要素を取得
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            // レイキャスト結果をデバッグ出力
            Debug.Log($"Raycast Results Count: {results.Count}");

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
        else
        {
            Debug.LogError("Camera.main is null. Unable to convert world position to screen position.");
        }
    }
}
