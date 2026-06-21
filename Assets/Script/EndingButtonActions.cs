using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ending 씬의 버튼 동작을 처리합니다.
/// GoTitle 버튼에는 GoTitle(), RETRY 버튼에는 RetryGame()을 연결하세요.
/// </summary>
public class EndingButtonActions : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string titleSceneName = "Title";
    [SerializeField] private string gameSceneName = "Main";

    public void GoTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleSceneName);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}
