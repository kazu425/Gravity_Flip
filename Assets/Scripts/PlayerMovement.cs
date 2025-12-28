using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playRb;

    public float moveForce = 20f;   // 加速力
    public float maxSpeed = 7f;     // 最大速度
    public float jumpPower = 5f;

    void Start()
    {
        playRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // --- 重力方向 ---
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // --- 重力と平行でない基準軸 ---
        Vector3 refAxis = (Mathf.Abs(Vector3.Dot(up, Vector3.forward)) > 0.9f)
            ? Vector3.up
            : Vector3.forward;

        Vector3 right = Vector3.Cross(up, refAxis).normalized;
        Vector3 forward = Vector3.Cross(right, up).normalized;

        // --- WASD 入力 ---
        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.D)) input += right;
        if (Input.GetKey(KeyCode.A)) input -= right;
        if (Input.GetKey(KeyCode.W)) input += forward;
        if (Input.GetKey(KeyCode.S)) input -= forward;

        // --- 現在の速度 ---
        Vector3 vel = playRb.linearVelocity;

        // --- 入力がある場合：AddForce で加速 ---
        if (input != Vector3.zero)
        {
            playRb.AddForce(input.normalized * moveForce, ForceMode.Acceleration);
        }
        else
        {
            // --- 入力ゼロ：水平速度だけ0にする ---
            Vector3 verticalVel = Vector3.Project(vel, gravityDir);
            playRb.linearVelocity = verticalVel;
        }

        // --- 最大速度制限（ワールド座標のまま） ---
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
}