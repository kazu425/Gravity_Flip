using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f;
    public float jumpHeight = 2f;

    public Vector3 gravityDir = Vector3.down;   // 重力方向
    public float gravityStrength = 9.81f;       // 重力の大きさ

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- Ground Check ---
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            float downVel = Vector3.Dot(velocity, gravityDir);
            if (downVel > 0)
            {
                velocity -= gravityDir * downVel;
            }
        }

        // --- Movement Input (WASD のみ) ---
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.S)) v -= 1f;
        if (Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.A)) h -= 1f;

        Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, -gravityDir).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(Camera.main.transform.right,   -gravityDir).normalized;

        Vector3 move = (camForward * v + camRight * h);
        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // --- Rotation ---
        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move, -gravityDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity -= gravityDir * Mathf.Sqrt(jumpHeight * 2f * gravityStrength);
        }

        // --- Apply Gravity ---
        velocity += gravityDir * gravityStrength * Time.deltaTime;

        // --- Final Move ---
        Vector3 finalMove = move * moveSpeed + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }
}