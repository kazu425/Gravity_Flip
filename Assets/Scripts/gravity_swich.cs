using UnityEngine;

public class gravity_swich : MonoBehaviour
{

    private void OCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        Destroy(gameObject);
         ChangeGravity(new Vector3(9.81f, 0, 0));
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
