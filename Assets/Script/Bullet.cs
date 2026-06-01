using UnityEngine;

/// <summary>
/// 정면(transform.forward)으로 일직선 이동하는 총알.
/// Player에 닿으면 카메라 시야를 흔들고(CameraShake), 옵션으로 데미지를 준 뒤 사라집니다.
/// 트리거 충돌을 위해 Collider(Is Trigger) + Kinematic Rigidbody가 필요합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifeTime = 6f;

    [Header("On Hit Player")]
    [Tooltip("명중 시 흔들림 지속(초)")]
    [SerializeField] private float shakeDuration = 0.4f;
    [Tooltip("명중 시 흔들림 세기(각도)")]
    [SerializeField] private float shakeMagnitude = 7f;
    [Tooltip("명중 시 데미지(0이면 데미지 없음)")]
    [SerializeField] private int damage = 0;

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {
        if (lifeTime > 0f)
            Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void SetSpeed(float value)
    {
        speed = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        HitPlayer(other.transform);
        Destroy(gameObject);
    }

    private void HitPlayer(Transform playerTransform)
    {
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);

        if (damage > 0)
        {
            PlayerHealth health = playerTransform.GetComponent<PlayerHealth>();
            if (health == null)
                health = playerTransform.GetComponentInParent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
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
