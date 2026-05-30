using UnityEngine;

/// <summary>
/// BreakWall이 부서진 뒤에만 플레이어를 탐지·추적합니다.
/// Enemy 오브젝트에 붙이고, Player에는 Tag "Player"를 지정하세요.
/// </summary>
public class EnemyChase : MonoBehaviour
{
    [Header("Gate")]
    [Tooltip("체크 시 BreakWall이 파괴될 때까지 추적하지 않음")]
    [SerializeField] private bool waitForBreakWall = true;
    [Tooltip("비우면 씬에서 이름이 BreakWall인 오브젝트를 찾습니다")]
    [SerializeField] private BreakWall breakWallGate;

    [Header("Detection")]
    [SerializeField] private float detectRange = 12f;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackUpForce = 3f;
    [SerializeField] private float knockbackHitDistance = 1.5f;
    [SerializeField] private float knockbackCooldown = 0.6f;

    private Transform player;
    private bool detectionEnabled;
    private bool isChasing;
    private float knockbackCooldownTimer;

    private void OnEnable()
    {
        BreakWall.OnBroken += OnBreakWallDestroyed;
    }

    private void OnDisable()
    {
        BreakWall.OnBroken -= OnBreakWallDestroyed;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[EnemyChase] Tag가 'Player'인 오브젝트를 찾을 수 없습니다.");

        if (!waitForBreakWall)
        {
            detectionEnabled = true;
            return;
        }

        if (BreakWall.HasBeenBroken)
        {
            detectionEnabled = true;
            return;
        }

        if (breakWallGate == null)
        {
            GameObject wallObj = GameObject.Find("BreakWall");
            if (wallObj != null)
                breakWallGate = wallObj.GetComponent<BreakWall>();
        }

        if (breakWallGate == null)
        {
            Debug.LogWarning("[EnemyChase] BreakWall을 찾지 못했습니다. 추적을 바로 시작합니다.");
            detectionEnabled = true;
            return;
        }

        detectionEnabled = false;
    }

    private void OnBreakWallDestroyed()
    {
        detectionEnabled = true;
    }

    private void Update()
    {
        if (!detectionEnabled || player == null)
            return;

        if (knockbackCooldownTimer > 0f)
            knockbackCooldownTimer -= Time.deltaTime;

        float distance = GetFlatDistanceToPlayer();

        if (distance <= detectRange)
            isChasing = true;
        else
            isChasing = false;

        if (isChasing)
            TryKnockbackPlayer(distance);

        if (!isChasing || distance <= stopDistance)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        direction.Normalize();
        transform.position += direction * chaseSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void TryKnockbackPlayer(float distance)
    {
        if (knockbackCooldownTimer > 0f)
            return;

        if (distance > knockbackHitDistance)
            return;

        PlayerKnockback knockback = player.GetComponent<PlayerKnockback>();
        if (knockback == null)
            knockback = player.GetComponentInParent<PlayerKnockback>();

        if (knockback == null)
            return;

        Vector3 direction = player.position - transform.position;
        knockback.ApplyKnockback(direction, knockbackForce, knockbackUpForce);
        knockbackCooldownTimer = knockbackCooldown;
    }

    private float GetFlatDistanceToPlayer()
    {
        return Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.position.x, 0f, player.position.z));
    }

    private void OnDrawGizmosSelected()
    {
        if (!detectionEnabled && Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
