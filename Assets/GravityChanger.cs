using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    void Update()
    {
        // --- 重力方向切り替え ---
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeGravity(new Vector3(0, 9.81f, 0));

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeGravity(new Vector3(0, -9.81f, 0));

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeGravity(new Vector3(9.81f, 0, 0));

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeGravity(new Vector3(-9.81f, 0, 0));

        if (Input.GetKeyDown(KeyCode.LeftShift))
            ChangeGravity(new Vector3(0, 0, 9.81f));

        if (Input.GetKeyDown(KeyCode.RightShift))
            ChangeGravity(new Vector3(0, 0, -9.81f));
    }

    void ChangeGravity(Vector3 newGravity)
    {
        // --- 重力方向を変更 ---
        Physics.gravity = newGravity;

        // --- シーン内のすべての Rigidbody を取得 ---
        Rigidbody[] bodies = FindObjectsOfType<Rigidbody>();

        foreach (var rb in bodies)
        {
            // --- 速度を保持（再投影を防ぐ）---
            rb.linearVelocity = rb.linearVelocity;
        }
    }
}