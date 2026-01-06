using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class RunnerGrapple : NetworkBehaviour
{
    [Header("Grapple Settings")]
    public float grappleSpeed = 20f;     // 飛ぶ速さ
    public float grappleDuration = 0.25f; // 飛び続ける時間
    public float cooldownTime = 3f;       // クールタイム

    float cooldownTimer = 0f;
    float grappleTimer = 0f;

    bool isGrappling = false;
    Vector3 grappleDirection;

    CharacterController controller;
    Camera cam;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    void Update()
    {
        //なんかあれうん自分だけを動かすようにするやつらしい
        if (!IsOwner) return;
        
        // --- クールタイム ---
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // --- グラップル開始 ---
        if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0f && !isGrappling)
        {
            StartGrapple();
        }

        // --- グラップル中の移動 ---
        if (isGrappling)
        {
            GrappleMove();
        }
    }

    void StartGrapple()
    {
        if (cam == null) return;

        grappleDirection = cam.transform.forward.normalized;
        grappleTimer = grappleDuration;
        isGrappling = true;
        cooldownTimer = cooldownTime;
    }

    void GrappleMove()
    {
        if (grappleTimer > 0f)
        {
            Vector3 move = grappleDirection * grappleSpeed * Time.deltaTime;
            controller.Move(move);
            grappleTimer -= Time.deltaTime;
        }
        else
        {
            isGrappling = false;
        }
    }
}

