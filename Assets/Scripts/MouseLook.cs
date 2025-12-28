using UnityEngine;

public class GravityMouseLook : MonoBehaviour
{
    public float sensitivity = 200f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        // 重力方向
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // 重力に対して水平な回転軸（Y軸の代わり）
        Vector3 refAxis = (Mathf.Abs(Vector3.Dot(up, Vector3.forward)) > 0.9f)
            ? Vector3.up
            : Vector3.forward;

        Vector3 right = Vector3.Cross(up, refAxis).normalized;
        Vector3 forward = Vector3.Cross(right, up).normalized;

        // プレイヤーを重力に対して水平に回転
        Quaternion rotation = Quaternion.AngleAxis(mouseX, up);
        transform.rotation = rotation * transform.rotation;
    }
}