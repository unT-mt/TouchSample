using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SensorTouchHandler : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    private EventSystem eventSystem;

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
            var pointerEventData = new PointerEventData(eventSystem)
            {
                position = sensorTouch.position
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    AnimateButton(button);
                    break;
                }
            }
        }
    }

    private void AnimateButton(Button button)
    {
        ColorBlock colors = button.colors;
        colors.pressedColor = Color.red;  // 押下時の色を設定
        button.colors = colors;
    }
}
