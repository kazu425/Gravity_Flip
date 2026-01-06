using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class NetworkUI : MonoBehaviour
{
    public GameObject uiRoot;          // Canvas 全体
    public InputField ipInputField;    // Client が入力する IP
    public Text ipDisplayText;         // Host の IP を表示する Text

    public void StartHost()
    {
        // Host のローカルIPを自動取得
        string hostIP = GetLocalIPAddress();

        // UI を消す前に表示だけ更新
        if (ipDisplayText != null)
        {
            ipDisplayText.text = "Your IP: " + hostIP;
        }

        // UI を先に消す（InputField が破棄される前に処理済み）
        HideUI();

        // Transport に設定
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = hostIP;

        Debug.Log("Host IP: " + hostIP);

        // Host 開始
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        // UI が消える前に IP を取得
        string ip = ipInputField.text;

        // UI を先に消す
        HideUI();

        // Transport に設定
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ip;

        Debug.Log("Client connecting to: " + ip);

        // Client 開始
        NetworkManager.Singleton.StartClient();
    }

    void HideUI()
    {
        if (uiRoot != null)
            uiRoot.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Host のローカルIPv4を取得
    public static string GetLocalIPAddress()
    {
        foreach (var ni in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ni.AddressFamily == AddressFamily.InterNetwork)
            {
                return ni.ToString(); // 例: 192.168.0.12
            }
        }
        return "127.0.0.1"; // fallback
    }
}