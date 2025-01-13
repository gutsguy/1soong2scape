using UnityEngine;

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
        animator.SetBool("character_nearby", false);
    }

    public void OpenDoor(){
        animator.SetBool("character_nearby", true);  // 값을 토글
        bool isCharacterNearby = animator.GetBool("character_nearby");  // 현재 값 가져오기
        Debug.Log($"character_nearby 상태: {isCharacterNearby}");
    }

    public void CloseDoor(){
        animator.SetBool("character_nearby", false);  // 값을 토글
        bool isCharacterNearby = animator.GetBool("character_nearby");  // 현재 값 가져오기
        Debug.Log($"character_nearby 상태: {isCharacterNearby}");
    }
}
