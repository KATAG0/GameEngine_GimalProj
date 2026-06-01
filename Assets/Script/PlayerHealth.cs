using UnityEngine;
using UnityEngine.UI;

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

    [Header("Respawn")]
    [Tooltip("부활 위치(Transform). 지정하면 이 위치를 우선 사용")]
    [SerializeField] private Transform respawnPoint;
    [Tooltip("직접 입력하는 부활 좌표. Respawn Point가 비어 있을 때 사용")]
    [SerializeField] private Vector3 respawnPosition;

    [Header("Early Respawn (키 미획득 시)")]
    [Tooltip("필요한 키(Orb) 개수. 이만큼 못 모은 상태에서 죽으면 아래 좌표로 스폰")]
    [SerializeField] private int requiredKeyCount = 3;
    [Tooltip("키를 다 모으기 전에 죽었을 때 스폰할 좌표(직접 입력)")]
    [SerializeField] private Vector3 earlyRespawnPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
        InitSlider();
    }

    private void InitSlider()
    {
        if (hpSlider == null)
            return;

        hpSlider.minValue = 0;
        hpSlider.maxValue = maxHealth;
        hpSlider.value = currentHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0)
            return;

        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateSlider();
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

        UpdateSlider();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (hpSlider != null)
            hpSlider.value = currentHealth;
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] 플레이어 사망! 부활 지점으로 돌아갑니다.");
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

        ResetHealth();
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
