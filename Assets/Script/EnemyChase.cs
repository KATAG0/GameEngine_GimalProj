using UnityEngine;

/// <summary>
/// 플레이어가 인식 범위 안에 들어오면 따라갑니다.
/// Enemy 오브젝트에 붙이고, Player 오브젝트에는 Tag "Player"를 지정하세요.
/// </summary>
public class EnemyChase : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectRange = 8f;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float stopDistance = 1.2f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackUpForce = 3f;
    [SerializeField] private float knockbackHitDistance = 1.5f;
    [SerializeField] private float knockbackCooldown = 0.6f;

    private Transform player;
    private bool isChasing;
    private float knockbackCooldownTimer;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("[EnemyChase] Tag가 'Player'인 오브젝트를 찾을 수 없습니다.");
    }

    private void Update()
    {
        if (player == null)
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
