using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent; // ConcurrentQueueを使うための名前空間
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

    [Header("Sensor Settings")]
    public float maxDistance = 10.0f;
    public int beamCount = 1080;

    private SensorTouchManager touchManager;
    public SensorTouchHandler sensorTouchHandler;

    // 非同期受信データのキュー
    private ConcurrentQueue<Vector2> positionQueue = new ConcurrentQueue<Vector2>();

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
                Vector2 parsedPosition = ParsePosition(Encoding.UTF8.GetString(result.Buffer));
                if (parsedPosition != Vector2.zero)
                {
                    // パース済みのpositionをキューに格納
                    positionQueue.Enqueue(parsedPosition);
                }
            }
        });
    }

    void Update()
    {
        // positionQueueにデータがある場合、取り出して処理
        while (positionQueue.TryDequeue(out Vector2 position))
        {
            Vector2 screenPosition = ConvertToScreenPosition(position);

            // SensorTouchオブジェクトを作成してHandlerに渡す
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

            if (Mathf.Approximately(distance, maxDistance)) continue;

            Vector2 beamDetection = CalculateBeamDetection(id, distance);
            if (IsWithinRange(beamDetection))
            {
                //Debug.Log($"Obstacle detected at beam {id} with distance {distance} meters.");
                return beamDetection;
            }
        }
        return Vector2.zero;
    }

    private Vector2 CalculateBeamDetection(int id, float distance)
    {
        float angle = Mathf.Lerp(-135f, 135f, (float)id / (beamCount - 1));
        float radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radian) * distance, Mathf.Cos(radian) * distance);
    }

    private bool IsWithinRange(Vector2 beamDetection)
    {
        return beamDetection.x < frontRight.x && beamDetection.y < frontRight.y &&
               beamDetection.x > frontLeft.x  && beamDetection.y < frontLeft.y  &&
               beamDetection.x > backLeft.x   && beamDetection.y > backLeft.y   &&
               beamDetection.x < backRight.x  && beamDetection.y > backRight.y;
    }

    private Vector2 ConvertToScreenPosition(Vector2 position)
    {
        // 検出範囲の中央座標と範囲サイズ（高さと幅）
        Vector2 rangeCenter = new Vector2((frontRight.x + frontLeft.x + backRight.x + backLeft.x) / 4,
                                        (frontRight.y + frontLeft.y + backRight.y + backLeft.y) / 4);
        float rangeWidth = Mathf.Abs(frontRight.x - frontLeft.x);  // 検出範囲の幅
        float rangeHeight = Mathf.Abs(frontRight.y - backRight.y); // 検出範囲の高さ

        // `position` を検出範囲内の正規化された座標に変換
        float normalizedX = (position.x - (rangeCenter.x - rangeWidth / 2)) / rangeWidth;
        float normalizedY = (position.y - (rangeCenter.y - rangeHeight / 2)) / rangeHeight;

        // スクリーン解像度へのマッピング
        float screenX = normalizedX * 1920f;
        float screenY = normalizedY * 1080f;

        return new Vector2(screenX, screenY);
    }

    private void OnDestroy()
    {
        udpClient.Close();
    }
}
