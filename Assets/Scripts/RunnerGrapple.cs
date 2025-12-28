using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerGrapple : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grapplePower = 50f;   // 飛距離・勢い
    public float cooldownTime = 3f;    // クールタイム（秒）

    float cooldownTimer = 0f;

    Rigidbody rb;
    Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update()
    {
        // --- クールタイム処理 ---
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // --- 右クリックでグラップル ---
        if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0f)
        {
            ActivateGrapple();
        }
    }

    void ActivateGrapple()
    {
        if (cam == null) return;

        // 視点方向（重力に依存しない）
        Vector3 grappleDir = cam.transform.forward.normalized;

        // 現在の速度をリセット（暴走防止）
        rb.linearVelocity = Vector3.zero;

        // 視点方向に瞬間加速
        rb.AddForce(grappleDir * grapplePower, ForceMode.Impulse);

        // クールタイム開始
        cooldownTimer = cooldownTime;
    }
}
