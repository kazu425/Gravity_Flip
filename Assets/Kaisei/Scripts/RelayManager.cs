using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // -------------------------
    // 部屋を作る（ホスト）
    // -------------------------
    public async Task<string> CreateRelayRoom(int maxPlayers = 4)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"[Relay] JoinCode: {joinCode}");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            // UI 切り替え
            FindObjectOfType<UIManager>().OnConnected();

            return joinCode;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Relay Create Error: {e}");
            return null;
        }
    }

    // -------------------------
    // JoinCode で参加（クライアント）
    // -------------------------
    public async Task<bool> JoinRelayRoom(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            // UI 切り替え
            FindObjectOfType<UIManager>().OnConnected();


            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Relay Join Error: {e}");
            return false;
        }
    }
}