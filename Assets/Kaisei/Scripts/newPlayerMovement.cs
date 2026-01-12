using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class newPlayerMovement : NetworkBehaviour
{
    [Header("鬼ごっこ設定")]
    public NetworkVariable<bool> isOni = new(false);
    public NetworkVariable<bool> isDead = new(false);

    public GameObject model;
    public GameObject oniLabel;

[Header("重力設定")]
public Vector3 gravityDirection = Vector3.down;   // 重力方向
public float gravityStrength = 9.81f;             // 重力の強さ


    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("ドッジ設定")]
    public float dodgeSpeed = 20f;
    public float dodgeDuration = 0.25f;
    public float dodgeCooldown = 3f;

    float dodgeTimer;
    float dodgeCooldownTimer;
    bool isDodging;
    Vector3 dodgeDirection;

    CharacterController controller;
    Vector3 velocity;
    bool isGrounded;
    Animator anim;

    // ============================
    // Network Spawn
    // ============================
    public override void OnNetworkSpawn()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        if (IsServer)
        {
            isOni.Value = IsHost;
        }

        oniLabel.SetActive(isOni.Value);

        if (IsOwner)
        {
            Camera.main.GetComponent<CameraFollow>().target = transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ============================
    // Update
    // ============================
    void Update()
    {
        if (!IsOwner) return;

        // ---- 死亡中は完全停止 ----
        if (isDead.Value)
        {
            controller.enabled = false;
            anim.SetFloat("Speed", 0);
            return;
        }
        else if (!controller.enabled)
        {
            controller.enabled = true;
            ResetCamera();
        }

        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        if (isDodging)
        {
            DodgeMove();
            return;
        }

        NormalMove();
    }

    // ============================
    // 通常移動
    // ============================
    void NormalMove()
{
    // ===== 接地判定 =====
    isGrounded = controller.isGrounded;

    // ===== 入力による移動 =====
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    Vector3 move = new Vector3(h, 0, v);
    move = Camera.main.transform.TransformDirection(move);
    move = Vector3.ProjectOnPlane(move, gravityDirection); // ← 重力方向に合わせて地面に沿わせる

    controller.Move(move * moveSpeed * Time.deltaTime);

    // ===== 回転 =====
    if (move.magnitude > 0.1f)
    {
        Quaternion rot = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    // ======== 重力処理（ここに入れる）========
    if (isGrounded && Vector3.Dot(velocity, gravityDirection) > 0)
    {
        velocity = Vector3.ProjectOnPlane(velocity, gravityDirection);
    }

    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        velocity += -gravityDirection * Mathf.Sqrt(jumpHeight * 2f * gravityStrength);
    }

    velocity += gravityDirection * gravityStrength * Time.deltaTime;

    controller.Move(velocity * Time.deltaTime);
}

    // ============================
    // ドッジ
    // ============================
    void StartDodge()
    {
        dodgeDirection = Camera.main.transform.forward;
        dodgeDirection.y = 0f;
        dodgeDirection.Normalize();

        dodgeTimer = dodgeDuration;
        dodgeCooldownTimer = dodgeCooldown;
        isDodging = true;

        anim.SetTrigger("Dodge");
    }

    void DodgeMove()
    {
        if (dodgeTimer > 0f)
        {
            controller.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            dodgeTimer -= Time.deltaTime;
        }
        else
        {
            isDodging = false;
        }
    }

    // ============================
    // 攻撃（Server）
    // ============================
    [ServerRpc]
    void AttackServerRpc()
    {
        float range = 3f;
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            newPlayerMovement target = hit.GetComponent<newPlayerMovement>();
            if (target != null && target != this && !target.isDead.Value)
            {
                target.Kill();
            }
        }
    }

    // ============================
    // 死亡 & 復活（Server主導）
    // ============================
    void Kill()
    {
        isDead.Value = true;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(3f);

        transform.position = new Vector3(0, 1, 0);
        velocity = Vector3.zero;

        isDead.Value = false;
    }

    // ============================
    // Camera Reset
    // ============================
    void ResetCamera()
    {
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        cam.target = transform;
        cam.SnapToTarget();
    }
}
