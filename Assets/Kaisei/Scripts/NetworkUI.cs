using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    [Header("UI References")]
    public InputField joinCodeInput;   // JoinCode を入力する欄
    public Text joinCodeDisplay;       // Host が生成した JoinCode を表示する欄

    public Button hostButton;
    public Button joinButton;

    private void Start()
    {
        hostButton.onClick.AddListener(OnHostClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private async void OnHostClicked()
    {
        string joinCode = await RelayManager.Instance.CreateRelayRoom();

        if (!string.IsNullOrEmpty(joinCode))
        {
            joinCodeDisplay.text = "Join Code: " + joinCode;
            FindObjectOfType<UIManager>().OnConnected();
        }
        else
        {
            joinCodeDisplay.text = "Error creating room";
        }
    }

    private async void OnJoinClicked()
    {
        string code = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(code))
        {
            joinCodeDisplay.text = "Enter Join Code";
            return;
        }

        bool success = await RelayManager.Instance.JoinRelayRoom(code);

        if (success)
        {
            FindObjectOfType<UIManager>().OnConnected();
        }
        else
        {
            joinCodeDisplay.text = "Join Failed";
        }
    }
}