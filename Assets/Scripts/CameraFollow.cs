using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);
    public float mouseSensitivity = 2f;
    public float distance = 5f;
    public float minY = -90f;
    public float maxY = 90f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        
    }

    void LateUpdate()
    {
        if (!target) return;

        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

        Vector3 desiredPosition = target.position - (rotation * Vector3.forward * distance);
        transform.position = desiredPosition + new Vector3(0, offset.y, 0);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public void SnapToTarget()
    {
    if (!target) return;

    // カメラの回転をターゲット基準にリセット
    rotationX = target.eulerAngles.y;
    rotationY = 0f;

    Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

    Vector3 desiredPosition = target.position - (rotation * Vector3.forward * distance);
    transform.position = desiredPosition + new Vector3(0, offset.y, 0);

    transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}

