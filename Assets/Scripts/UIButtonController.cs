using UnityEngine;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
    public Button targetButton;  // Assign the target UIButton in the inspector
    private SensorTouch currentTouch;
    private RectTransform buttonRectTransform;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        buttonRectTransform = targetButton.GetComponent<RectTransform>();
    }

    // Method to update touch data, called from SensorTouchGenerator
    public void UpdateTouchPosition(Vector2 screenPosition)
    {
        // Check if we have an active touch, otherwise initialize it
        if (currentTouch == null)
        {
            currentTouch = new SensorTouch(0, screenPosition, Vector2.zero, 0, TouchPhase.Began, TouchType.Direct, true);
        }
        else
        {
            // Update existing touch data
            currentTouch.Update(screenPosition, Time.deltaTime);
        }

        // Move the button based on the touch position
        ControlUIButton(screenPosition);
    }

    private void ControlUIButton(Vector2 screenPosition)
    {
        // Convert screen position to local position relative to the button’s RectTransform
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(buttonRectTransform.parent as RectTransform, screenPosition, mainCamera, out localPoint);

        // Check if the touch is within the button's bounds
        if (RectTransformUtility.RectangleContainsScreenPoint(buttonRectTransform, screenPosition, mainCamera))
        {
            // Perform button action if the touch is within bounds
            Debug.Log("Button Touched!");
            targetButton.onClick.Invoke();
        }
    }

    // Method to clear touch data, e.g., on touch end
    public void ClearTouch()
    {
        currentTouch = null;
    }
}
