using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // 캐릭터 이동 속도
    public float mouseSensitivity = 2f;  // 마우스 감도

    private float xRotation = 0f;  // 상하 회전 각도
    public Transform cameraTransform;  // 플레이어가 사용하는 카메라

    void Start()
    {
        // 커서를 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 카메라 상하 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 상하 회전 제한
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 플레이어 좌우 회전
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // 카메라가 바라보는 방향 기준으로 이동
        float moveX = Input.GetAxis("Horizontal");  // A, D 입력
        float moveZ = Input.GetAxis("Vertical");  // W, S 입력

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        moveDirection.Normalize();  // 방향 벡터를 정규화
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
