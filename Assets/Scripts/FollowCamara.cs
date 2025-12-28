using UnityEngine;

public class GravityFollowCamera : MonoBehaviour
{
    public Transform target;          // プレイヤー
    public Vector3 offset;            // 相対位置
    public float smoothPos = 0.1f;    // 位置のスムーズ
    public float smoothRot = 5f;      // 回転のスムーズ
    public Vector3 gravityDir = Vector3.down; // 現在の重力方向

    void LateUpdate()
    {
        // --- 位置の追従 ---
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothPos);

        // --- 回転の追従 ---
        // プレイヤーの forward を基準にしつつ、重力方向を up として回転を作る
        Quaternion targetRot = Quaternion.LookRotation(
            target.forward,          // カメラの前方向
            -gravityDir              // 重力の逆方向 = 上方向
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            smoothRot * Time.deltaTime
        );
    }
}