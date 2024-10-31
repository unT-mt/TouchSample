using UnityEngine;

public class CoordinateConverter
{
    private Vector2 rangeCenter;
    private float rangeWidth;
    private float rangeHeight;
    private Vector2 screenCenter;
    private Vector2 screenResolution;

    public CoordinateConverter(Vector2 frontRight, Vector2 frontLeft, Vector2 backRight, Vector2 backLeft)
    {
        // 検出範囲の中央と範囲サイズを計算
        rangeCenter = new Vector2((frontRight.x + frontLeft.x + backRight.x + backLeft.x) / 4,
                                  (frontRight.y + frontLeft.y + backRight.y + backLeft.y) / 4);
        rangeWidth = Mathf.Abs(frontRight.x - frontLeft.x);
        rangeHeight = Mathf.Abs(frontRight.y - backRight.y);

        // スクリーンの中央座標と解像度を取得
        screenResolution = new Vector2(Screen.width, Screen.height);
        screenCenter = screenResolution / 2f;
    }

    public Vector2 ToScreenPosition(Vector2 worldPosition)
    {
        // 検出範囲内での位置を0〜1に正規化
        float normalizedX = (worldPosition.x - (rangeCenter.x - rangeWidth / 2)) / rangeWidth;
        float normalizedY = (worldPosition.y - (rangeCenter.y - rangeHeight / 2)) / rangeHeight;

        // スクリーン座標に変換
        float screenX = normalizedX * screenResolution.x;
        float screenY = normalizedY * screenResolution.y;

        return new Vector2(screenX, screenY);
    }
}
