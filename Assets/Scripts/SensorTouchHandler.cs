using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SensorTouchHandler : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    private EventSystem eventSystem;

    private GameObject currentButton = null;  // 現在の障害物の位置にあるボタン
    //private bool isPointerDown = false;       // ボタン内に「タッチ」が継続されているかどうかを追跡

    void Start()
    {
        eventSystem = EventSystem.current;

        if (Camera.main == null)
        {
            Debug.LogError("Main camera is not found. Ensure the camera has the 'MainCamera' tag.");
        }
    }

    public void ProcessSensorTouch(SensorTouch sensorTouch)
    {
        if (Camera.main != null)
        {
            // PointerEventDataを生成し、障害物の位置を設定
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = sensorTouch.position
            };

            // Raycast結果のリストを用意
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            // 現在障害物が重なっているボタンを取得
            GameObject detectedButton = null;
            foreach (var result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    detectedButton = result.gameObject;
                    break;
                }
            }

            // 新しいボタンに入った場合、または異なるボタンに入った場合
            if (detectedButton != null && currentButton != detectedButton)
            {
                // 前回のボタンから抜ける場合、`OnPointerExit`を発火
                if (currentButton != null)
                {
                    ExecuteEvents.Execute(currentButton, pointerEventData, ExecuteEvents.pointerExitHandler);
                }

                // 新しいボタンに対して`OnPointerEnter`と`OnPointerDown`を発火
                ExecuteEvents.Execute(detectedButton, pointerEventData, ExecuteEvents.pointerEnterHandler);
                ExecuteEvents.Execute(detectedButton, pointerEventData, ExecuteEvents.pointerDownHandler);
                
                currentButton = detectedButton;
                //isPointerDown = true;
            }
            else if (detectedButton == null && currentButton != null)
            {
                // ボタン内にあった障害物が消えた場合、`OnPointerUp`を発火
                ExecuteEvents.Execute(currentButton, pointerEventData, ExecuteEvents.pointerUpHandler);
                
                // ボタンの追跡解除
                currentButton = null;
                //isPointerDown = false;
            }
            else if (detectedButton != currentButton && currentButton != null)
            {
                // ボタンから範囲外に出た場合、`OnPointerExit`のみを発火
                ExecuteEvents.Execute(currentButton, pointerEventData, ExecuteEvents.pointerExitHandler);
                
                currentButton = null;
                //isPointerDown = false;
            }
        }
        else
        {
            Debug.LogError("Camera.main is null. Unable to convert world position to screen position.");
        }
    }
}
