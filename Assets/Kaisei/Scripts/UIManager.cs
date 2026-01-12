using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Groups")]
    public GameObject mainUI;       // HostButton, JoinButton, JoinCodeInput の親
    public GameObject joinCodeUI;   // JoinCodeText の親

    // 接続後に呼ぶ
    public void OnConnected()
    {
        mainUI.SetActive(false);    // ボタン類を非表示
        joinCodeUI.SetActive(true); // JoinCode は残す
    }

    // 切断時に呼ぶ（必要なら）
    public void OnDisconnected()
    {
        mainUI.SetActive(true);
        joinCodeUI.SetActive(true);
    }
}