using UnityEngine;

/// <summary>
/// Enemy 루트에 붙입니다.
/// 1) requireBreakWall=true면 BreakWall 파괴 후에만 추적
/// 2) Inspector의 Chase Target(씬의 Player)을 detectRange 안에서 추적
/// 3) 가까우면 넉백 + 데미지
/// </summary>
public class EnemyChase : MonoBehaviour
{
    [Header("Chase Target")]
    [Tooltip("Player 프리팹 또는 씬의 Player 오브젝트를 드래그")]
    [SerializeField] private GameObject chaseTarget;

    [Header("Detection")]
    [SerializeField] private float detectRange = 12f;
    [SerializeField] private bool requireBreakWall = true;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackUpForce = 3f;
    [SerializeField] private float knockbackHitDistance = 1.5f;
    [SerializeField] private float knockbackCooldown = 0.6f;

    [Header("Damage")]
    [SerializeField] private int damage = 1;

    private Transform playerTarget;
    private bool detectionEnabled;
    private float knockbackCooldownTimer;

    private void OnEnable()
    {
        BreakWall.OnBroken += EnableDetection;
    }

    private void OnDisable()
    {
        BreakWall.OnBroken -= EnableDetection;
    }

    private void Start()
    {
        ResolveChaseTarget();
        detectionEnabled = !requireBreakWall || BreakWall.HasBeenBroken;
    }

    private void Update()
    {
        if (!detectionEnabled)
            return;

        if (playerTarget == null)
            ResolveChaseTarget();

        if (playerTarget == null)
            return;

        if (knockbackCooldownTimer > 0f)
            knockbackCooldownTimer -= Time.deltaTime;

        float distance = GetFlatDistanceToTarget();
        if (distance > detectRange)
            return;

        TryKnockbackPlayer(distance);

        if (distance > stopDistance)
            MoveToTarget();
    }

    private void EnableDetection()
    {
        detectionEnabled = true;
        Debug.Log("[EnemyChase] BreakWall 파괴 → 추적 활성화");
    }

    private void MoveToTarget()
    {
        Vector3 direction = playerTarget.position - transform.position;
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

        PlayerKnockback knockback = playerTarget.GetComponentInParent<PlayerKnockback>();
        if (knockback == null)
            return;

        Vector3 direction = playerTarget.position - transform.position;
        bool knockedBack = knockback.ApplyKnockback(direction, knockbackForce, knockbackUpForce);
        knockbackCooldownTimer = knockbackCooldown;

        if (knockedBack)
            DealDamageToPlayer();
    }

    private void DealDamageToPlayer()
    {
        PlayerHealth health = playerTarget.GetComponentInParent<PlayerHealth>();
        if (health != null)
            health.TakeDamage(damage);
    }

    private float GetFlatDistanceToTarget()
    {
        return Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(playerTarget.position.x, 0f, playerTarget.position.z));
    }

    private void ResolveChaseTarget()
    {
        if (chaseTarget == null)
        {
            playerTarget = null;
            Debug.LogError("[EnemyChase] Chase Target이 비어 있습니다. Player 프리팹/오브젝트를 Inspector에 할당하세요.", this);
            return;
        }

        // 씬에 올라간 Player 인스턴스를 직접 넣은 경우
        if (chaseTarget.scene.IsValid())
        {
            playerTarget = chaseTarget.transform;
            return;
        }

        // Project 창의 Player 프리팹을 넣은 경우 → 씬에서 같은 이름의 활성 오브젝트 탐색
        GameObject sceneInstance = GameObject.Find(chaseTarget.name);
        if (sceneInstance != null)
        {
            playerTarget = sceneInstance.transform;
            return;
        }

        playerTarget = null;
        Debug.LogError($"[EnemyChase] 씬에서 Player 인스턴스('{chaseTarget.name}')를 찾지 못했습니다.", this);
    }

    private void OnDrawGizmos()
    {
        DrawDetectRangeGizmo(0.35f);
    }

    private void OnDrawGizmosSelected()
    {
        DrawDetectRangeGizmo(1f);
    }

    private void DrawDetectRangeGizmo(float alpha)
    {
        float radius = Mathf.Max(detectRange, 1f);

        if (!Application.isPlaying)
            Gizmos.color = new Color(1f, 1f, 0f, alpha);
        else if (detectionEnabled)
            Gizmos.color = new Color(1f, 1f, 0f, alpha);
        else
            Gizmos.color = new Color(0.6f, 0.6f, 0.6f, alpha);

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
