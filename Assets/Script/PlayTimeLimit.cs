using TMPro;
using UnityEngine;

/// <summary>
/// 플레이 제한 시간(기본 120초)을 세고 UI 텍스트로 남은 시간을 표시합니다.
/// Canvas 아래 TextMeshPro에 연결하세요.
/// </summary>
public class PlayTimeLimit : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float timeLimitSeconds = 120f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [Tooltip("예: 남은 시간 {0} → 남은 시간 01:59")]
    [SerializeField] private string displayFormat = "남은 시간 {0}";

    [Header("Time Up")]
    [SerializeField] private bool stopPlayerOnTimeUp = true;
    [SerializeField] private string timeUpMessage = "시간 종료!";

    private float remainingTime;
    private bool isRunning = true;
    private bool timeUpTriggered;
    private PlayerController playerController;

    public float RemainingTime => remainingTime;
    public bool IsRunning => isRunning;

    private void Start()
    {
        remainingTime = timeLimitSeconds;
        UpdateDisplay();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<PlayerController>();

        if (timerText == null)
            Debug.LogWarning("[PlayTimeLimit] Timer Text가 연결되지 않았습니다.");
    }

    private void Update()
    {
        if (!isRunning || timeUpTriggered)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            OnTimeUp();
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (timerText == null)
            return;

        timerText.text = string.Format(displayFormat, FormatTime(remainingTime));
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.CeilToInt(Mathf.Max(0f, seconds));
        int minutes = total / 60;
        int secs = total % 60;
        return $"{minutes:00}:{secs:00}";
    }

    private void OnTimeUp()
    {
        timeUpTriggered = true;
        isRunning = false;

        if (timerText != null)
            timerText.text = timeUpMessage;

        if (stopPlayerOnTimeUp && playerController != null)
            playerController.enabled = false;
    }

    public void AddTime(float seconds)
    {
        if (timeUpTriggered)
            return;

        remainingTime += seconds;
        UpdateDisplay();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        if (!timeUpTriggered)
            isRunning = true;
    }
}
