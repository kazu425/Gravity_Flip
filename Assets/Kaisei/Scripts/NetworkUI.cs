using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class NetworkUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private InputField joinCodeInput;   // JoinCode 入力欄
    [SerializeField] private Text       joinCodeDisplay; // Host が生成した JoinCode 表示欄

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    // 任意：ステータス表示（別テキストがあると便利）
    [SerializeField] private Text statusText;

    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();

        if (hostButton == null || joinButton == null)
            Debug.LogWarning("[NetworkUI] Buttons がインスペクタで未設定です。");
        if (joinCodeInput == null || joinCodeDisplay == null)
            Debug.LogWarning("[NetworkUI] InputField/Text がインスペクタで未設定です。");
        if (_uiManager == null)
            Debug.LogWarning("[NetworkUI] シーン内に UIManager が見つかりません。");
    }

    private void Start()
    {
        hostButton?.onClick.AddListener(OnHostClicked);
        joinButton?.onClick.AddListener(OnJoinClicked);
    }

    private async void OnHostClicked()
    {
        SetInteractable(false);
        SetStatus("部屋を作成中…");

        try
        {
            string joinCode = await RelayManager.Instance.CreateRelayRoom();

            if (!string.IsNullOrEmpty(joinCode))
            {
                if (joinCodeDisplay) joinCodeDisplay.text = $"Join Code: {joinCode}";
                SetStatus("部屋を作成しました。参加者を待機中…");
                _uiManager?.OnConnected(); // 画面切替など
            }
            else
            {
                if (joinCodeDisplay) joinCodeDisplay.text = "部屋の作成に失敗しました";
                SetStatus("作成に失敗しました。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NetworkUI] Host 失敗: {ex}");
            if (joinCodeDisplay) joinCodeDisplay.text = "部屋の作成でエラーが発生しました";
            SetStatus("作成中にエラーが発生しました。");
        }
        finally
        {
            SetInteractable(true);
        }
    }

    private async void OnJoinClicked()
    {
        SetInteractable(false);
        SetStatus("参加中…");

        try
        {
            string code = joinCodeInput ? joinCodeInput.text.Trim() : string.Empty;

            if (string.IsNullOrEmpty(code))
            {
                if (joinCodeDisplay) joinCodeDisplay.text = "Join Code を入力してください";
                SetStatus("Join Code を入力してください。");
                return;
            }

            bool success = await RelayManager.Instance.JoinRelayRoom(code);

            if (success)
            {
                SetStatus("参加に成功しました。");
                _uiManager?.OnConnected();
            }
            else
            {
                if (joinCodeDisplay) joinCodeDisplay.text = "参加に失敗しました";
                SetStatus("参加できませんでした。コードやネットワークをご確認ください。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NetworkUI] Join 失敗: {ex}");
            if (joinCodeDisplay) joinCodeDisplay.text = "参加に失敗しました";
            SetStatus("参加中にエラーが発生しました。");
        }
        finally
        {
            SetInteractable(true);
        }
    }

    private void SetInteractable(bool value)
    {
        if (hostButton) hostButton.interactable = value;
        if (joinButton) joinButton.interactable = value;
        if (joinCodeInput) joinCodeInput.interactable = value;
    }

    private void SetStatus(string msg)
    {
        if (statusText) statusText.text = msg;
        else Debug.Log($"[NetworkUI] {msg}");
    }
}
