using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    [Header("UI Button Settings")]
    public Button targetButton;

    private const int BeamCount = 1081;
    private SensorTouchManager touchManager;

    void Start()
    {
        InitializeUdpClient();
        touchManager = new SensorTouchManager();
    }

    private void InitializeUdpClient()
    {
        udpClient = new UdpClient(listenPort);
        Task.Run(async () =>
        {
            while (true)
            {
                var result = await udpClient.ReceiveAsync();
                ProcessReceivedData(Encoding.UTF8.GetString(result.Buffer));
            }
        });
    }

    private void ProcessReceivedData(string data)
    {
        // 検出された beamDetection 座標を格納するリスト
        List<Vector2> detectedPositions = new List<Vector2>();

        string[] entries = data.Split(';');
        foreach (string entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry)) continue;

            string[] values = entry.Split(',');
            if (values.Length != 2) continue;

            int id = int.Parse(values[0]);
            float distance = float.Parse(values[1]);

            Vector2 beamDetection = CalculateBeamDetection(id, distance);
            if (IsWithinRange(beamDetection))
            {
                Debug.Log($"Obstacle detected at beam {id} with distance {distance} meters.");
                detectedPositions.Add(beamDetection);  // リストに追加
            }
        }

        // 座標の平均値を計算
        Vector2 averagePosition = CalculateAveragePosition(detectedPositions);
        Debug.Log($"Average Detection Position: {averagePosition}");

        touchManager.GenerateTouch(averagePosition);  // 平均位置をタッチ位置として生成
    }

    private Vector2 CalculateAveragePosition(List<Vector2> positions)
    {
        if (positions.Count == 0) return Vector2.zero;

        Vector2 sum = Vector2.zero;
        foreach (var position in positions)
        {
            sum += position;
        }
        return sum / positions.Count;
    }

    private Vector2 CalculateBeamDetection(int id, float distance)
    {
        // 中心を0度、右端を135度、左端を-135度とするように角度を設定
        float angle = Mathf.Lerp(-135f, 135f, (float)id / (BeamCount - 1));
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radian) * distance, Mathf.Cos(radian) * distance);
    }

    private bool IsWithinRange(Vector2 beamDetection)
    {
        return (beamDetection.x < frontRight.x && beamDetection.y < frontRight.y &&
                beamDetection.x > frontLeft.x && beamDetection.y < frontLeft.y &&
                beamDetection.x > backLeft.x && beamDetection.y > backLeft.y &&
                beamDetection.x < backRight.x && beamDetection.y > backRight.y);
    }

    private void OnDestroy()
    {
        udpClient.Close();
    }
}
