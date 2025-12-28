using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    public PlayerMovement player; // ← Inspector でプレイヤーをセット

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SetGravity(new Vector3(0, 1, 0));

        if (Input.GetKeyDown(KeyCode.DownArrow))
            SetGravity(new Vector3(0, -1, 0));

        if (Input.GetKeyDown(KeyCode.RightArrow))
            SetGravity(new Vector3(1, 0, 0));

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetGravity(new Vector3(-1, 0, 0));

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SetGravity(new Vector3(0, 0, 1));

        if (Input.GetKeyDown(KeyCode.RightShift))
            SetGravity(new Vector3(0, 0, -1));
    }

    void SetGravity(Vector3 dir)
    {
        // プレイヤーの重力方向と強さを直接更新
        player.gravityDir = dir.normalized;
        player.gravityStrength = 9.81f;
    }
}