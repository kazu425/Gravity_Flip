using UnityEngine;

public class GravityMouseLook : MonoBehaviour
{
    [Tooltip("上下回転を担当する CameraPivot を割り当ててください")]
    public Transform cameraPivot;

    [Tooltip("マウス感度")]
    public float sensitivity = 200f;

    public float Pitch { get; private set; } = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // 重力方向（下）→ up ベクトル（上）
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // --- Yaw（左右回転） ---
        transform.rotation = Quaternion.AngleAxis(mouseX, up) * transform.rotation;

        // --- Pitch（上下回転） ---
        Pitch -= mouseY;
        Pitch = Mathf.Clamp(Pitch, -80f, 80f);

        if (cameraPivot != null)
        {
            // 位置は絶対に触らない。localRotation だけ変更。
            cameraPivot.localRotation = Quaternion.Euler(Pitch, 0f, 0f);
        }
    }
}