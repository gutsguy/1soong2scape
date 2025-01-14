using UnityEngine;
using Photon.Pun;  // Photon 네트워크 사용

public class HideLocalPlayerMeshes : MonoBehaviourPun
{
    private Animator _animator;
    private bool _isDancing = false;

    void Start()
    {
        _animator = GetComponent<Animator>();

        // 로컬 플레이어라면 메쉬 렌더러 비활성화
        if (photonView.IsMine)  // PhotonView로 자신의 캐릭터인지 확인
        {
            Debug.Log("로컬 플레이어가 올바르게 인식되었습니다!");
            ToggleRenderers(false); // 초기에는 렌더러 비활성화
        }
    }

    void Update()
    {
        if (_animator != null)
        {
            // "isDancing" 애니메이션 상태 확인
            _isDancing = _animator.GetBool("isDancing1") ||
                         _animator.GetBool("isDancing2") ||
                         _animator.GetBool("isDancing3") ||
                         _animator.GetBool("isDancing4") ||
                         _animator.GetBool("isDancing5") ||
                         _animator.GetBool("isDancing6") ||
                         _animator.GetBool("isDancing7") ||
                         _animator.GetBool("isDancing8") ||
                         _animator.GetBool("isDancing9");

            ToggleRenderers(_isDancing); // 춤을 추면 렌더러 활성화, 아니면 비활성화
        }
    }

    private void ToggleRenderers(bool enabled)
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = enabled;  // Mesh Renderer 활성화/비활성화
        }
        SkinnedMeshRenderer[] skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
        {
            renderer.enabled = enabled;
        }
    }
}
