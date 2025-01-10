using UnityEngine;
using Photon.Pun;

public class NetworkPlayerController : MonoBehaviour
{
    private PhotonView photonView;  // PhotonView를 통한 네트워크 제어

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        // 자신이 소유한 플레이어만 제어
        if (!photonView.IsMine) return;

        // ThirdPersonController의 이동 처리
        HandleMovement();
    }

    private void HandleMovement()
    {
        // 이동 및 입력 처리: 기존 ThirdPersonController 스크립트 호출
        Debug.Log("Handling player movement for local player...");
        // 여기에 실제 이동 처리 코드를 작성할 수 있습니다.
    }
}
