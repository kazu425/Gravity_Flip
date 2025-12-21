using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    void Update()
    {
        // --- 重力方向切り替え ---
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Physics.gravity = new Vector3(0, 9.81f, 0);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            Physics.gravity = new Vector3(0, -9.81f, 0);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            Physics.gravity = new Vector3(9.81f, 0, 0);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Physics.gravity = new Vector3(-9.81f, 0, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            Physics.gravity = new Vector3(0, 0, 9.81f);

        if (Input.GetKeyDown(KeyCode.RightShift))
            Physics.gravity = new Vector3(0, 0, -9.81f);
    }
}