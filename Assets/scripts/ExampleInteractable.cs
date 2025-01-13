using UnityEngine;

public class ExampleInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log($"{gameObject.name}와 상호작용했습니다!");
        // 상호작용 로직: 예를 들어 오브젝트 색 변경
    }
}
