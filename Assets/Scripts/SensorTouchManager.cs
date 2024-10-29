using System.Collections.Generic;
using UnityEngine;

public class SensorTouchManager
{
    public event System.Action<SensorTouch> OnTouchBegan;
    public event System.Action<SensorTouch> OnTouchEnded;

    private Dictionary<int, SensorTouch> activeTouches = new Dictionary<int, SensorTouch>();
    private int nextFingerId = 0;

    public void GenerateTouch(Vector2 position)
    {
        int fingerId = nextFingerId++;
        var touch = new SensorTouch(fingerId, position, Vector2.zero, 0, TouchPhase.Began, TouchType.Direct, true);
        activeTouches[fingerId] = touch;
        OnTouchBegan?.Invoke(touch);
    }

    public void EndTouch(int fingerId)
    {
        if (activeTouches.TryGetValue(fingerId, out var touch))
        {
            touch.phase = TouchPhase.Ended;
            OnTouchEnded?.Invoke(touch);
            activeTouches.Remove(fingerId);
        }
    }
}
