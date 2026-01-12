using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    public newPlayerMovement player; // Inspector でプレイヤーをセット

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SetGravity(new Vector3(0, 1, 0));

        if (Input.GetKeyDown(KeyCode.X))
            SetGravity(new Vector3(0, -1, 0));

        if (Input.GetKeyDown(KeyCode.C))
            SetGravity(new Vector3(1, 0, 0));

        if (Input.GetKeyDown(KeyCode.V))
            SetGravity(new Vector3(-1, 0, 0));

        if (Input.GetKeyDown(KeyCode.B))
            SetGravity(new Vector3(0, 0, 1));

        if (Input.GetKeyDown(KeyCode.N))
            SetGravity(new Vector3(0, 0, -1));
    }

    void SetGravity(Vector3 dir)
    {
        // 正規化して重力方向を更新
        player.gravityDirection = dir.normalized;

        // 重力の強さを設定
        player.gravityStrength = 9.81f;

        Debug.Log("Gravity changed to: " + player.gravityDirection);
    }
}