using UnityEngine;
using Photon.Pun;  // Photon 네트워크 사용

public class HideLocalPlayerMeshes : MonoBehaviourPun
{
    void Start()
    {
        // 로컬 플레이어라면 메쉬 렌더러 비활성화
        if (photonView.IsMine)  // PhotonView로 자신의 캐릭터인지 확인
        {
            Debug.Log("로컬 플레이어가 올바르게 인식되었습니다!");
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.enabled = false;  // 로컬 캐릭터의 Mesh Renderer를 비활성화
            }
            SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                renderer.enabled = false;
            }
        }
    }
}
