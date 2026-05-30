using UnityEngine;

/// <summary>
/// 굴러 내려오는 구 장애물. Rigidbody + Sphere Collider 필요.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RollingBallObstacle : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackUpForce = 3f;

    [Header("Lifetime")]
    [SerializeField] private float destroyAfterSeconds = 7f;

    private void Start()
    {
        if (destroyAfterSeconds > 0f)
            Destroy(gameObject, destroyAfterSeconds);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        PlayerKnockback knockback = collision.collider.GetComponent<PlayerKnockback>();
        if (knockback == null)
            knockback = collision.collider.GetComponentInParent<PlayerKnockback>();
        if (knockback == null)
            return;

        Vector3 direction = collision.transform.position - transform.position;
        knockback.ApplyKnockback(direction, knockbackForce, knockbackUpForce);
    }
}
