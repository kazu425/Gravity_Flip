using UnityEngine;

public class player : MonoBehaviour
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
        // 現在の重力方向
        Vector3 gravityDir = Physics.gravity.normalized;
        Vector3 up = -gravityDir;

        // 重力と平行でない基準軸
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

        // --- 速度制御 ---
        Vector3 horizontalVel = input.normalized * moveSpeed;

        // 重力方向の速度成分は維持
        Vector3 verticalVel = Vector3.Project(playRb.linearVelocity, gravityDir);

        playRb.linearVelocity = horizontalVel + verticalVel;

        // --- ジャンプ ---
        if (Input.GetKeyDown(KeyCode.Space))
            playRb.AddForce(up * jumppower, ForceMode.Impulse);

        // --- 重力方向切り替え ---
        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeGravity(new Vector3(0, 9.81f, 0));
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeGravity(new Vector3(0, -9.81f, 0));
        if (Input.GetKeyDown(KeyCode.RightArrow)) ChangeGravity(new Vector3(9.81f, 0, 0));
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ChangeGravity(new Vector3(-9.81f, 0, 0));
        if (Input.GetKeyDown(KeyCode.LeftShift)) ChangeGravity(new Vector3(0, 0, 9.81f));
        if (Input.GetKeyDown(KeyCode.RightShift)) ChangeGravity(new Vector3(0, 0, -9.81f));
    }

    // ---------------------------------------------------------
    // 重力変更：速度は一切変更しない
    // ---------------------------------------------------------
    void ChangeGravity(Vector3 newGravity)
    {
        Physics.gravity = newGravity;
    }
}