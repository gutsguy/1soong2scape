using UnityEngine;
using Photon.Pun;

public class MicDoor : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();  // Animator 컴포넌트를 가져옵니다.
        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }
        animator.SetBool("character_nearby", true); // 지워야할지도
    }

    [PunRPC]  // RPC 함수 추가
    public void OpenDoorNetwork()
    {
        animator.SetBool("character_nearby", true);
        Debug.Log("Door opened (network synchronized)");
    }

    [PunRPC]
    public void CloseDoorNetwork()
    {
        animator.SetBool("character_nearby", false);
        Debug.Log("Door closed (network synchronized)");
    }
}
