using UnityEngine;

public class MoveController : MonoBehaviour
{
    public GameObject cube;  // 移動対象のCubeオブジェクト
    public float moveDistance = 1.0f;  // ボタンを押したときに移動する距離

    // Cubeを上方向に移動
    public void MoveUp()
    {
        if (cube != null)
        {
            cube.transform.Translate(Vector3.up * moveDistance);
        }
    }

    // Cubeを下方向に移動
    public void MoveDown()
    {
        if (cube != null)
        {
            cube.transform.Translate(Vector3.down * moveDistance);
        }
    }

    // Cubeを左方向に移動
    public void MoveLeft()
    {
        if (cube != null)
        {
            cube.transform.Translate(Vector3.left * moveDistance);
        }
    }

    // Cubeを右方向に移動
    public void MoveRight()
    {
        if (cube != null)
        {
            cube.transform.Translate(Vector3.right * moveDistance);
        }
    }
}
