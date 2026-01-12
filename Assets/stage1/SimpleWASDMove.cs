using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleWASDMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 1.8f;
    public float gravity = -9.81f;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- 接地判定 ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // 地面に吸い付ける
        }

        // --- WASD移動 ---
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);

        // カメラ基準移動
        if (Camera.main != null)
        {
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0f;
        }

        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- ジャンプ ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        // --- 重力 ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
