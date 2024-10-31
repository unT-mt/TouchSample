using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;

public class SensorTouchGenerator : MonoBehaviour
{
    [Header("UDP Settings")]
    public int listenPort = 5000;
    private UdpClient udpClient;

    [Header("Detection Range")]
    public Vector2 frontRight = new Vector2(0.96f, 1.28f);
    public Vector2 frontLeft = new Vector2(-0.96f, 1.28f);
    public Vector2 backLeft = new Vector2(-0.96f, 0.20f);
    public Vector2 backRight = new Vector2(0.96f, 0.20f);

    private CoordinateConverter coordinateConverter;
    private ConcurrentQueue<Vector2> positionQueue = new ConcurrentQueue<Vector2>();

    public SensorTouchHandler sensorTouchHandler;

    void Start()
    {
        coordinateConverter = new CoordinateConverter(frontRight, frontLeft, backRight, backLeft);
        InitializeUdpClient();
    }

    private void InitializeUdpClient()
    {
        udpClient = new UdpClient(listenPort);
        Task.Run(async () =>
        {
            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                Vector2 parsedPosition = ParsePosition(Encoding.UTF8.GetString(result.Buffer));
                if (parsedPosition != Vector2.zero)
                {
                    positionQueue.Enqueue(parsedPosition);
                }
            }
        });
    }

    void Update()
    {
        while (positionQueue.TryDequeue(out Vector2 position))
        {
            Vector2 screenPosition = coordinateConverter.ToScreenPosition(position);
            SensorTouch sensorTouch = new SensorTouch(0, screenPosition, Vector2.zero, 0, TouchPhase.Began, TouchType.Direct, true);
            sensorTouchHandler.ProcessSensorTouch(sensorTouch);
        }
    }

    private Vector2 ParsePosition(string data)
    {
        string[] entries = data.Split(';');
        foreach (string entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry)) continue;

            string[] values = entry.Split(',');
            if (values.Length != 2) continue;

            int id = int.Parse(values[0]);
            float distance = float.Parse(values[1]);

            if (Mathf.Approximately(distance, 10.0f)) continue;

            return CalculateBeamDetection(id, distance);
        }
        return Vector2.zero;
    }

    private Vector2 CalculateBeamDetection(int id, float distance)
    {
        float angle = Mathf.Lerp(-135f, 135f, (float)id / 1080);
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Sin(radian) * distance, Mathf.Cos(radian) * distance);
    }
}
