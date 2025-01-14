using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;  // 캐릭터 프리팹을 연결할 변수
    public CinemachineVirtualCamera cinemachineCamera;  // 카메라 참조


    void Start()
    {
        Debug.Log("Connecting to Photon Cloud...");
        PhotonNetwork.ConnectUsingSettings();  // Photon Cloud 연결
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Cloud!");
        PhotonNetwork.JoinOrCreateRoom("HelloRoom", new Photon.Realtime.RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogError("Disconnected from Photon Cloud: " + cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        SendHelloMessage();  // 방에 입장하면 Hello 메시지를 전송합니다.

        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber;

        Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(30, 0, 0),   // 첫 번째 플레이어 스폰 위치
        new Vector3(35, 0, 17),   // 두 번째 플레이어 스폰 위치
    };

        // 최대 인덱스를 넘지 않도록 보정
        Vector3 spawnPosition = spawnPositions[(playerIndex - 1) % spawnPositions.Length];
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", spawnPosition, Quaternion.identity);


        // 카메라 설정: 자신이 소유한 플레이어일 경우 카메라 연결
        if (player.GetComponent<PhotonView>().IsMine)
        {
            cinemachineCamera.Follow = player.transform.Find("PlayerCameraRoot");
            cinemachineCamera.LookAt = player.transform.Find("PlayerCameraRoot");
        }
    }

    [PunRPC]
    public void ReceiveHelloMessage()
    {
        Debug.Log("Hello from another player!");
    }

    public void SendHelloMessage()
    {
        if (photonView != null)
        {
            photonView.RPC("ReceiveHelloMessage", RpcTarget.All);  // 같은 방에 있는 모든 플레이어에게 메시지 전송
        }
        else
        {
            Debug.LogError("PhotonView is not attached!");
        }
    }
}
