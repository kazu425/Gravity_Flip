using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelayUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text joinCodeDisplay;

    public async void OnClickCreateRoom()
    {
        string code = await RelayManager.Instance.CreateRelayRoom();
        if (joinCodeDisplay != null)
            joinCodeDisplay.text = "JoinCode: " + code;
    }

    public async void OnClickJoinRoom()
    {
        string code = joinCodeInput.text;
        if (string.IsNullOrEmpty(code))
            return;

        await RelayManager.Instance.JoinRelayRoom(code);
    }
}