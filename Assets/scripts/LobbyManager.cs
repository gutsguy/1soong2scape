using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // 게임 시작 버튼 클릭 시 호출되는 함수
    public void OnStartGameButton()
    {
        Debug.Log("게임 시작 버튼 클릭됨!");
        SceneManager.LoadScene("mainScene");
    }

    public void OnExitGameButton()
    {
        Debug.Log("게임 종료 버튼 클릭됨!");
        Application.Quit();  // 애플리케이션 종료
    }
}