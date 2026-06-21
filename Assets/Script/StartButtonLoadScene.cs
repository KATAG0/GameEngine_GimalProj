using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// START 버튼에서 호출해 지정한 씬으로 이동합니다.
/// Button의 OnClick 이벤트에 LoadScene()을 연결하세요.
/// </summary>
public class StartButtonLoadScene : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("이동할 씬 이름. Build Settings에 등록된 이름과 같아야 함")]
    [SerializeField] private string sceneName = "Main";

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
