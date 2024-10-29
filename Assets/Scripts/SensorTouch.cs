using UnityEngine;

public class SensorTouch
{
    public int fingerId { get; set; }
    public Vector2 position { get; set; }
    public Vector2 deltaPosition { get; set; }
    public float deltaTime { get; set; }
    public TouchPhase phase { get; set; }
    public TouchType type { get; set; }
    public bool isPrimary { get; set; }

    public SensorTouch(int fingerId, Vector2 position, Vector2 deltaPosition, float deltaTime, TouchPhase phase, TouchType type, bool isPrimary)
    {
        this.fingerId = fingerId;
        this.position = position;
        this.deltaPosition = deltaPosition;
        this.deltaTime = deltaTime;
        this.phase = phase;
        this.type = type;
        this.isPrimary = isPrimary;
    }

    // 更新メソッドを追加
    public void Update(Vector2 newPosition, float newDeltaTime)
    {
        this.deltaPosition = newPosition - this.position;
        this.position = newPosition;
        this.deltaTime = newDeltaTime;
    }
}