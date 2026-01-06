using UnityEngine;
using Unity.Netcode;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public bool isOni; // 仮：Inspector で手動で鬼設定してもOK

    public GameObject model; // 見た目だけ

    public float moveSpeed = 5f;
    public float rotateSpeed = 720f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
        // Host のプレイヤーだけ鬼にする
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

        if (IsOwner)
        {
            // シーンのメインカメラを取得
            Camera mainCam = Camera.main;

            // CameraFollow に自分を渡す
            mainCam.GetComponent<CameraFollow>().target = this.transform;

            // カーソルロック（必要なら）
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
         //なんかあれうん自分だけを動かすようにするやつらしい
        if (!IsOwner) return;

        

        // 既存の移動・ジャンプの処理はそのまま下にあるとして…

        // 左クリックで攻撃
        if (isOni && Input.GetMouseButtonDown(0))
        {
         AttackServerRpc();
     }

        // ここに移動やジャンプの処理が続く

        
        // --- Ground Check ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 地面に張り付く感じに
        }

        // --- Movement Input ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);

        // カメラ基準の方向に変換
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0f;

        controller.Move(move * moveSpeed * Time.deltaTime);

        // --- 回転（動いてるときだけ） ---
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        // --- Jump ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

       

        // --- Apply Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    [ServerRpc]
    private void AttackServerRpc()
    {
    float range = 3f;
    Vector3 center = transform.position;

    Collider[] hits = Physics.OverlapSphere(center, range);

    foreach (var hit in hits)
    {
        PlayerMovement player = hit.GetComponent<PlayerMovement>();

        if (player != null && player != this)
        {
            Debug.Log($"[Server] {player.OwnerClientId} を攻撃範囲内で発見");

            // サーバーから「そのプレイヤー自身」に向けて RPC を送る
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

    [ClientRpc]
    public void KillClientRpc(ClientRpcParams rpcParams = default)
    {

    Debug.Log("[Client] やられた！");
    // 見た目だけ消す（NetworkObject は残す）
    model.SetActive(false);

    if(IsOwner)
    {
        StartCoroutine(Respawn());
    }     

    }



    
    private IEnumerator Respawn()
    {
    yield return new WaitForSeconds(3f);
    model.SetActive(true);
    yield return null; // 1フレーム待つことで LateUpdate を回避

    transform.position = new Vector3(0, 1, 0);

    // カメラを即座にジャンプさせる
    if (IsOwner)
    {
        Camera.main.GetComponent<CameraFollow>().SnapToTarget();
    }
    }
}

    

