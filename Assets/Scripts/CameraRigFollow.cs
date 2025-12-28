using UnityEngine;

public class CameraRigFollow : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        // カメラリグをプレイヤーの位置に追従させる
        transform.position = player.position;
    }
}