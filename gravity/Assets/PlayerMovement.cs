using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playRb;
    public float moveSpeed = 5f;
    public float jumppower = 5f;

    void Start()
    {
        playRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- 現在の重力方向 ---
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

        // --- 水平速度 ---
        Vector3 horizontalVel = input.normalized * moveSpeed;

        // --- 重力方向の速度成分は維持 ---
        Vector3 verticalVel = Vector3.Project(playRb.linearVelocity, gravityDir);

        // --- 合成して最終速度 ---
        playRb.linearVelocity = horizontalVel + verticalVel;

        // --- ジャンプ ---
        if (Input.GetKeyDown(KeyCode.Space))
            playRb.AddForce(up * jumppower, ForceMode.Impulse);
    }
}