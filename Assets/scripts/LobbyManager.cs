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
}