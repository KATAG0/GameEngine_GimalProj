using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 클리어 버튼/목표 지점. 플레이어가 닿거나(터치) 펀치로 때리면(Hit)
/// 타이머를 멈추고 "Clear!" 메시지를 띄웁니다.
/// 버튼 오브젝트에 붙이세요. (터치로 쓰려면 Collider의 Is Trigger를 켜 두세요)
/// </summary>
public class ClearTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("멈출 타이머. 비우면 씬에서 자동으로 찾음")]
    [SerializeField] private PlayTimeLimit timer;
    [Tooltip("'Clear!' 텍스트(TMP). 클리어 시 이 텍스트가 켜지고 내용이 바뀜")]
    [SerializeField] private TMP_Text clearText;
    [Tooltip("clearText 대신/함께 켤 UI 오브젝트(패널 등). 선택")]
    [SerializeField] private GameObject clearUIObject;
    [SerializeField] private string clearMessage = "Clear!";

    [Header("Behavior")]
    [Tooltip("플레이어가 닿기만 해도 클리어")]
    [SerializeField] private bool triggerOnPlayerTouch = true;
    [Tooltip("체크 시 HP가 1 이상인 상태에서만 Clear 처리")]
    [SerializeField] private bool requireAliveForClear = true;

    [Header("Ending")]
    [Tooltip("클리어 후 이동할 엔딩 씬 이름")]
    [SerializeField] private string endingSceneName = "Ending";

    private bool cleared;

    private void Awake()
    {
        if (timer == null)
            timer = FindObjectOfType<PlayTimeLimit>();

        if (clearUIObject != null)
            clearUIObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnPlayerTouch)
            return;

        if (!IsPlayer(other))
            return;

        TriggerClear(other.transform);
    }

    /// <summary>
    /// 펀치(공격)로 호출됩니다. PlayerController.TryPunch가 사용합니다.
    /// </summary>
    public void Hit()
    {
        GameObject player = GameObject.FindWithTag("Player");
        TriggerClear(player != null ? player.transform : null);
    }

    public void TriggerClear(Transform player)
    {
        if (cleared)
            return;

        cleared = true;

        if (timer != null)
            timer.StopTimer();

        if (clearText != null)
            clearText.text = clearMessage;

        if (clearUIObject != null)
            clearUIObject.SetActive(true);

        if (CanClear(player))
        {
            EndingResult.SetClear();
            Debug.Log("[ClearTrigger] Clear! 타이머 정지");
        }
        else
        {
            EndingResult.SetGameOver();
            Debug.Log("[ClearTrigger] HP가 닳은 상태로 도착하여 GameOver 처리");
        }

        SceneManager.LoadScene(endingSceneName);
    }

    private bool CanClear(Transform player)
    {
        if (!requireAliveForClear)
            return true;

        if (player == null)
            return false;

        PlayerHealth health = player.GetComponentInParent<PlayerHealth>();
        if (health == null)
            health = player.root.GetComponentInChildren<PlayerHealth>();

        return health != null && health.CurrentHealth > 0;
    }

    private static bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player"))
            return true;

        Transform current = other.transform.parent;
        while (current != null)
        {
            if (current.CompareTag("Player"))
                return true;
            current = current.parent;
        }

        return false;
    }
}
