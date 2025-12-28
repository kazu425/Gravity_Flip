using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playRb;

    public float moveForce = 20f;   // 加速力
    public float maxSpeed = 7f;     // 最大速度
    public float jumpPower = 5f;

    public float mouseSensitivity = 200f;

    void Start()
    {
        playRb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        // --- 重力方向 ---
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // --- カメラの向きに合わせた移動軸 ---
        Transform cam = Camera.main.transform;

        // カメラ forward を重力に対して水平に投影
        Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, gravityDir).normalized;
        Vector3 camRight = Vector3.Cross(up, camForward).normalized;

        // --- WASD 入力 ---
        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.D)) input += camRight;
        if (Input.GetKey(KeyCode.A)) input -= camRight;
        if (Input.GetKey(KeyCode.W)) input += camForward;
        if (Input.GetKey(KeyCode.S)) input -= camForward;

        // --- 現在の速度 ---
        Vector3 vel = playRb.linearVelocity;

        // --- 入力がある場合：加速 ---
        if (input != Vector3.zero)
        {
            playRb.AddForce(input.normalized * moveForce, ForceMode.Acceleration);
        }
        else
        {
            // 入力ゼロ：水平速度だけ0にする
            Vector3 verticalVel = Vector3.Project(vel, gravityDir);
            playRb.linearVelocity = verticalVel;
        }

        // --- 最大速度制限 ---
        if (playRb.linearVelocity.magnitude > maxSpeed)
        {
            playRb.linearVelocity = playRb.linearVelocity.normalized * maxSpeed;
        }

        // --- ジャンプ ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playRb.AddForce(up * jumpPower, ForceMode.Impulse);
        }
    }

    void Update()
    {
        // --- マウスでプレイヤーを水平回転（Yaw） ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // 重力方向を軸に回転
        Quaternion rot = Quaternion.AngleAxis(mouseX, up);
        transform.rotation = rot * transform.rotation;
    }
}