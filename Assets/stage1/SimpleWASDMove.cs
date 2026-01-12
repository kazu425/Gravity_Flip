using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleWASDMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A / D
        float v = Input.GetAxis("Vertical");   // W / S

        Vector3 move = new Vector3(h, 0f, v);

        // カメラがあればカメラ基準で移動
        if (Camera.main != null)
        {
            move = Camera.main.transform.TransformDirection(move);
            move.y = 0f;
        }

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
