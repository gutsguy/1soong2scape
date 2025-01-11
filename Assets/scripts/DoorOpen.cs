using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();  // Animator 컴포넌트를 가져옵니다.
        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))  // F 키 입력
        {
            bool isCharacterNearby = animator.GetBool("character_nearby");  // 현재 값 가져오기
            animator.SetBool("character_nearby", !isCharacterNearby);  // 값을 토글
            Debug.Log($"character_nearby 상태: {!isCharacterNearby}");
        }
    }
}
