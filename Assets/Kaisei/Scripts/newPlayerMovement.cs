using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class newPlayerMovement : NetworkBehaviour
{
    [Header("鬼ごっこ設定")]
    public bool isOni;
    public GameObject model;
    public GameObject oniLabel;

    [Header("移動設定")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("ドッジ設定")]
    public float dodgeSpeed = 20f;
    public float dodgeDuration = 0.25f;
    public float dodgeCooldown = 3f;

    private float dodgeTimer = 0f;
    private float dodgeCooldownTimer = 0f;
    private bool isDodging = false;
    private Vector3 dodgeDirection;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private Animator anim;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (IsHost)
            {
                isOni = true;
                Debug.Log("[Server] Host を鬼に設定したよ");
            }
            else
            {
                isOni = false;
            }
        }

        oniLabel.SetActive(isOni);

        if (IsOwner)
        {
            Camera.main.GetComponent<CameraFollow>().target = this.transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        // --- クールタイム更新 ---
        if (dodgeCooldownTimer > 0f)
            dodgeCooldownTimer -= Time.deltaTime;

        // --- ドッジ中なら専用処理 ---
        if (isDodging)
        {
            DodgeMove();
            return; // ← 通常移動は止める
        }

        // --- 通常移動 ---
        NormalMove();
    }

    // ============================
    // 通常移動
    // ============================
    void NormalMove()
    {
        // --- Ground Check ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Movement Input ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0f;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- Rotation ---
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Animation ---
        anim.SetFloat("Speed", move.magnitude);
        anim.SetBool("IsGrounded", isGrounded);

        // --- Attack ---
        if (isOni && Input.GetMouseButtonDown(0))
        {
            AttackServerRpc();
            anim.SetTrigger("Attack");
        }

        // --- Dodge ---
        if (Input.GetMouseButtonDown(1) && dodgeCooldownTimer <= 0f)
        {
            StartDodge();
        }
    }

    // ============================
    // ドッジ開始
    // ============================
    void StartDodge()
    {
        dodgeDirection = Camera.main.transform.forward.normalized;
        dodgeTimer = dodgeDuration;
        isDodging = true;
        dodgeCooldownTimer = dodgeCooldown;

        anim.SetTrigger("Dodge");
    }

    // ============================
    // ドッジ移動
    // ============================
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
    // 攻撃 RPC
    // ============================
    [ServerRpc]
    private void AttackServerRpc()
    {
        float range = 3f;
        Vector3 center = transform.position;

        Collider[] hits = Physics.OverlapSphere(center, range);

        foreach (var hit in hits)
        {
            newPlayerMovement player = hit.GetComponent<newPlayerMovement>();

            if (player != null && player != this)
            {
                Debug.Log($"[Server] {player.OwnerClientId} を攻撃範囲内で発見");

                player.KillClientRpc(new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { player.OwnerClientId }
                    }
                });
            }
        }
    }

    // ============================
    // Kill RPC
    // ============================
    [ClientRpc]
    public void KillClientRpc(ClientRpcParams rpcParams = default)
    {
        Debug.Log("[Client] やられた！");
        model.SetActive(false);

        if (IsOwner)
            StartCoroutine(Respawn());
    }

    // ============================
    // Respawn
    // ============================
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f);

        model.SetActive(true);
        yield return null;

        transform.position = new Vector3(0, 1, 0);

        if (IsOwner)
            Camera.main.GetComponent<CameraFollow>().SnapToTarget();
    }
}