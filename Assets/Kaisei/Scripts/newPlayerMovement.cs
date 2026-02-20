using UnityEngine;
using Unity.Netcode;
using System.Collections;
using NUnit.Framework;

[RequireComponent(typeof(CharacterController))]
public class newPlayerMovement : NetworkBehaviour
{
    [Header("鬼ごっこ設定")]
    public NetworkVariable<bool> isOni = new(false);
    public NetworkVariable<bool> isDead = new(false);

    public GameObject model;
    public GameObject oniLabel;

    [Header("重力設定")]
    public Vector3 gravityDirection = Vector3.down; // GravityChanger から変更される
    public float gravityStrength = 9.81f;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f;
    public float jumpHeight = 2f;

    [Header("ドッジ設定")]
    public float dodgeSpeed = 20f;
    public float dodgeDuration = 0.25f;
    public float dodgeCooldown = 3f;

    float dodgeTimer;
    float dodgeCooldownTimer;
    bool isDodging;
    Vector3 dodgeDirection;

    CharacterController controller;
    public Vector3 velocity; // GravityChanger から触るので public
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
    // 接地判定（重力方向対応）
    // ============================
    // ===== 接地判定用設定 =====
[SerializeField] LayerMask groundMask = ~0;        // 地面レイヤーだけに絞る（必要に応じてInspectorで設定）
[SerializeField] float groundCheckDistance = 0.2f; // 足元からキャストする最大距離（短めが安定）
[SerializeField] float groundSnapDistance = 0.05f; // 微小な浮きを吸収する貼り付き距離（任意）

   bool CheckGround()
{
    if (controller == null) controller = GetComponent<CharacterController>();

    Vector3 gdir = gravityDirection.normalized; // 下（足側）
    float radius = controller.radius;
    float height = Mathf.Max(controller.height, radius * 2f);
    float skin   = controller.skinWidth;

    Vector3 center = controller.bounds.center;

    // 頭側(top)・足側(bottom)を gdir 基準で定義
    Vector3 top    = center - gdir * (height * 0.5f - radius - skin);
    Vector3 bottom = center + gdir * (height * 0.5f - radius - skin);

    // 自カプセルとの初期重なり回避のため、キャスト方向と逆に少し戻す
    const float castBack = 0.02f;
    Vector3 p1 = top    - gdir * castBack;
    Vector3 p2 = bottom - gdir * castBack;

    bool hit = Physics.CapsuleCast(
        p1,
        p2,
        radius * 0.95f,
        gdir,                               // 下向きへキャスト
        out RaycastHit hitInfo,
        groundCheckDistance,
        groundMask,
        QueryTriggerInteraction.Ignore
    );

    // 斜面が急すぎる場合は接地扱いしない（任意、必要なら有効）
    if (hit)
    {
        float slope = Vector3.Angle(hitInfo.normal, -gdir); // 地面法線と“上向き”の角度
        if (slope > controller.slopeLimit + 0.5f)
            hit = false;
    }

    // スナップ（上向きに飛んでない時だけ）
    // velocity の“下向き成分”がほぼゼロ以上（= 上昇していない）なら吸着
    if (hit && Vector3.Dot(velocity, -gdir) <= 0.01f)
    {
        float gap = hitInfo.distance;
        if (gap > 0f && gap < groundSnapDistance)
            controller.Move(gdir * gap); // 微小距離なので 2 回目の Move でも実害は小さい
    }

    return hit;
}


    // ============================
    // Update
    // ============================
   void Update()
{
    if (!IsOwner) return;

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

    // ★ 先に接地判定（スナップ込み）
    isGrounded = CheckGround();

    if (dodgeCooldownTimer > 0f)
        dodgeCooldownTimer -= Time.deltaTime;

    if (isDodging)
    {
        DodgeMove();
        return;
    }

    NormalMove();

    // ★ Move 後にもう一度接地判定（位置が変わるため）
    isGrounded = CheckGround();

    if (isGrounded)
    {
        Debug.Log(isGrounded);
    }
}

    
    // ============================
    // 通常移動（重力方向対応）
    // ============================
   void NormalMove()
{
    // ===== 入力 =====
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    Vector3 move = new Vector3(h, 0, v);
    move = Camera.main.transform.TransformDirection(move);
    move = Vector3.ProjectOnPlane(move, gravityDirection.normalized); // 正規化して使う
    move.Normalize();

    // ===== 回転 =====
    if (move.sqrMagnitude > 0.01f)
    {
        Quaternion rot = Quaternion.LookRotation(move, -gravityDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    Vector3 gdir = gravityDirection.normalized;

    // ===== 接地中の下向き速度をクリア（重力方向成分のみ）=====
    if (isGrounded)
    {
        float downSpeed = Vector3.Dot(velocity, gdir);
        if (downSpeed > 0f)
            velocity -= Vector3.Project(velocity, gdir);
    }

    // ===== ジャンプ =====
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        velocity += -gdir * Mathf.Sqrt(jumpHeight * 2f * gravityStrength);
        isGrounded = false; // 直後は空中
    }

    // ===== 重力 =====
    velocity += gdir * gravityStrength * Time.deltaTime;

    // ===== 実移動（Move は 1 回だけ）=====
    Vector3 finalMove = move * moveSpeed + velocity;
    controller.Move(finalMove * Time.deltaTime);
}
    // ============================
    // ドッジ
    // ============================
    void StartDodge()
    {
        dodgeDirection = Camera.main.transform.forward;
        dodgeDirection = Vector3.ProjectOnPlane(dodgeDirection, gravityDirection);
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
    // 死亡 & 復活
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