using UnityEngine;

public class GravityCameraFollow : MonoBehaviour
{
    public Transform target;       // 追尾対象（プレイヤーなど）
    public float followDistance = 5f;
    public float followHeight = 2f;
    public float positionSmooth = 5f;
    public float rotationSmooth = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // 重力方向
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // 重力と平行でない軸を選ぶ
        Vector3 refAxis = (Mathf.Abs(Vector3.Dot(up, Vector3.forward)) > 0.9f)
            ? Vector3.up
            : Vector3.forward;

        Vector3 right = Vector3.Cross(up, refAxis).normalized;
        Vector3 forward = Vector3.Cross(right, up).normalized;

        // カメラの理想位置（プレイヤーの後ろ＋上）
        Vector3 offset = up * followHeight - forward * followDistance;
        Vector3 targetPos = target.position + offset;

        // 位置を滑らかに追尾
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionSmooth);

        // カメラの forward を重力に対して水平に補正
        Vector3 camForward = Vector3.ProjectOnPlane(transform.forward, up).normalized;
        Quaternion targetRot = Quaternion.LookRotation(camForward, up);

        // 回転を滑らかに補正
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmooth);
    }
}