using TMPro;
using UnityEngine;

/// <summary>
/// Ending 씬의 결과 텍스트에 붙이세요.
/// 이전 씬에서 저장한 결과에 따라 "GameOver" 또는 "Clear!"를 표시합니다.
/// </summary>
public class EndingResultText : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    private void Awake()
    {
        if (resultText == null)
            resultText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (resultText != null)
            resultText.text = EndingResult.Message;
    }
}
