using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어 체력(HP)을 관리하고 UI Slider(HP바)에 표시합니다.
/// Player 오브젝트에 붙이고, hpSlider에 Canvas의 Slider를 연결하세요.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [Tooltip("HP바로 사용할 UI Slider")]
    [SerializeField] private Slider hpSlider;
    [Tooltip("HP를 숫자로 표시할 TextMeshPro 텍스트")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private string hpTextFormat = "HP: {0} / {1}";

    [Header("Respawn")]
    [Tooltip("부활 위치(Transform). 지정하면 이 위치를 우선 사용")]
    [SerializeField] private Transform respawnPoint;
    [Tooltip("직접 입력하는 부활 좌표. Respawn Point가 비어 있을 때 사용")]
    [SerializeField] private Vector3 respawnPosition;
    [Tooltip("사망 후 부활할 때 회복되는 HP")]
    [SerializeField] private int respawnHealth = 4;

    [Header("Early Respawn (키 미획득 시)")]
    [Tooltip("필요한 키(Orb) 개수. 이만큼 못 모은 상태에서 죽으면 아래 좌표로 스폰")]
    [SerializeField] private int requiredKeyCount = 3;
    [Tooltip("키를 다 모으기 전에 죽었을 때 스폰할 좌표(직접 입력)")]
    [SerializeField] private Vector3 earlyRespawnPosition;

    [Header("Game Over")]
    [Tooltip("HP가 0이 된 횟수가 이 값에 도달하면 게임오버 씬으로 이동")]
    [SerializeField] private int gameOverDeathCount = 2;
    [Tooltip("게임오버 시 이동할 씬 이름")]
    [SerializeField] private string gameOverSceneName = "Ending";

    private int deathCount;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;
    public bool IsFullHealth => currentHealth >= maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        InitHealthUI();
    }

    private void InitHealthUI()
    {
        if (hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = maxHealth;
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthUI();
        Debug.Log($"[PlayerHealth] 데미지 {amount} → 현재 HP: {currentHealth} / {maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (hpSlider != null)
            hpSlider.value = currentHealth;

        if (hpText != null)
            hpText.text = string.Format(hpTextFormat, currentHealth, maxHealth);
    }

    private void Die()
    {
        deathCount++;
        Debug.Log($"[PlayerHealth] 플레이어 사망 횟수: {deathCount} / {gameOverDeathCount}");

        if (deathCount >= gameOverDeathCount)
        {
            Debug.Log("[PlayerHealth] 사망 횟수 초과! 게임오버 씬으로 이동합니다.");
            EndingResult.SetGameOver();
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 destination = GetRespawnDestination();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = destination;
        }
        else
        {
            transform.position = destination;
        }

        currentHealth = Mathf.Clamp(respawnHealth, 1, maxHealth);
        UpdateHealthUI();
        Debug.Log($"[PlayerHealth] 부활 완료 → 위치 {destination}, HP {currentHealth}/{maxHealth}");
    }

    private Vector3 GetRespawnDestination()
    {
        if (!HasRequiredKeys())
        {
            Debug.Log("[PlayerHealth] 키 미획득 상태 사망 → Early Respawn 위치로 스폰");
            return earlyRespawnPosition;
        }

        return respawnPoint != null
            ? respawnPoint.position
            : respawnPosition;
    }

    private bool HasRequiredKeys()
    {
        PlayerOrbInventory inventory = GetComponent<PlayerOrbInventory>();
        if (inventory == null)
            inventory = GetComponentInParent<PlayerOrbInventory>();

        if (inventory == null)
            return true;

        return inventory.OrbCount >= requiredKeyCount;
    }

    /// <summary>
    /// 세이브포인트가 호출하여 부활 위치를 갱신합니다.
    /// </summary>
    public void SetRespawnPosition(Vector3 position)
    {
        respawnPoint = null;
        respawnPosition = position;
        Debug.Log($"[PlayerHealth] 부활 지점 갱신: {position}");
    }
}
