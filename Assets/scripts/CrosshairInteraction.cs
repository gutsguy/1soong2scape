using UnityEngine;

public class CrosshairInteraction : MonoBehaviour
{
    public float rayDistance = 1f;  // 상호작용 거리
    public LayerMask interactableLayer;  // 상호작용할 레이어

    private Camera mainCamera;
    private GameObject currentObject;

    void Start()
    {
        mainCamera = Camera.main;  // 메인 카메라 가져오기
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 왼쪽 클릭 시
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));  // 화면 중앙에서 레이 발사

            if (Physics.Raycast(ray, out hit, rayDistance, interactableLayer))
            {
                // 상호작용 인터페이스를 호출 (있는 경우)
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();  // 상호작용 메서드 실행
                }
            }
        }
    }
}
