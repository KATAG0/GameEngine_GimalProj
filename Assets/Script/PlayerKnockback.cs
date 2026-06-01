using UnityEngine;

/// <summary>
/// Enemy 등에서 밀쳐낼 때 사용합니다. Player 오브젝트에 붙이세요.
/// 한 번 넉백된 뒤 reKnockbackCooldown 초 동안은 다시 밀리지 않습니다.
/// </summary>
public class PlayerKnockback : MonoBehaviour
{
    [Header("Knockback Effect")]
    [SerializeField] private float knockbackDuration = 0.4f;

    [Header("Re-Knockback Cooldown")]
    [SerializeField] private float reKnockbackCooldown = 2f;

    private Rigidbody rb;
    private float knockbackTimer;
    private float reKnockbackCooldownTimer;

    public bool IsKnockedBack => knockbackTimer > 0f;
    public bool CanReceiveKnockback => reKnockbackCooldownTimer <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (knockbackTimer > 0f)
            knockbackTimer -= Time.deltaTime;

        if (reKnockbackCooldownTimer > 0f)
            reKnockbackCooldownTimer -= Time.deltaTime;
    }

    /// <summary>
    /// 넉백을 적용합니다. 쿨다운 때문에 실제로 밀려나면 true, 무시되면 false를 반환합니다.
    /// </summary>
    public bool ApplyKnockback(Vector3 direction, float force, float upForce)
    {
        if (rb == null)
            return false;

        if (!CanReceiveKnockback)
            return false;

        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            direction = transform.forward;

        direction.Normalize();

        Vector3 velocity = direction * force;
        velocity.y = upForce;
        rb.velocity = velocity;

        knockbackTimer = knockbackDuration;
        reKnockbackCooldownTimer = reKnockbackCooldown;
        return true;
    }
}
