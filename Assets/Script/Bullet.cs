using UnityEngine;

/// <summary>
/// 정면(transform.forward)으로 일직선 이동하는 총알.
/// - 그냥 맞으면: HP 1 감소 + 화면 흔들림 3초
/// - 펀치(반사 능력)로 튕겨내면: 데미지 0, 흔들림 없음 (반대로 날아감)
/// 트리거 충돌을 위해 Collider(Is Trigger) + Kinematic Rigidbody가 필요합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float speed = 18f;

    [Header("On Hit Player")]
    [Tooltip("명중 시 데미지")]
    [SerializeField] private int damage = 1;
    [Tooltip("명중 시 흔들림 지속(초)")]
    [SerializeField] private float shakeDuration = 2f;
    [Tooltip("명중 시 흔들림 세기(각도)")]
    [SerializeField] private float shakeMagnitude = 7f;

    [Header("Hit SFX")]
    [Tooltip("총알에 맞았을 때 재생할 첫 번째 소리")]
    [SerializeField] private AudioClip hitClip1;
    [Tooltip("총알에 맞았을 때 재생할 두 번째 소리")]
    [SerializeField] private AudioClip hitClip2;
    [Tooltip("피격 소리를 유지할 시간(초)")]
    [SerializeField] private float hitSoundDuration = 3f;
    [Range(0f, 1f)]
    [SerializeField] private float hitSoundVolume = 1f;

    [Header("Deflect")]
    [Tooltip("반사된 뒤 속도 배율")]
    [SerializeField] private float deflectSpeedMultiplier = 1.3f;
    [Tooltip("반사된 총알이 적에게 줄 데미지")]
    [SerializeField] private int deflectedEnemyDamage = 1;

    private bool deflected;

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
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
        // 이미 반사된 총알은 적에게만 데미지를 준다
        if (deflected)
        {
            TryDamageEnemy(other);
            return;
        }

        if (!IsPlayer(other))
            return;

        PlayerController controller = other.GetComponentInParent<PlayerController>();

        // 반사 능력자가 펀치(공격) 중이면 → 튕겨냄: 데미지 0, 흔들림 X
        if (controller != null && controller.IsDeflecting)
        {
            Deflect();
            return;
        }

        // 그냥 맞음 → 데미지 1 + 흔들림 3초
        HitPlayer(other.transform);
        Destroy(gameObject);
    }

    private void HitPlayer(Transform hitTransform)
    {
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);

        PlayHitSounds(hitTransform.position);

        if (damage <= 0)
            return;

        PlayerHealth health = hitTransform.GetComponentInParent<PlayerHealth>();
        if (health == null)
            health = hitTransform.root.GetComponentInChildren<PlayerHealth>();

        if (health != null)
            health.TakeDamage(damage);
        else
            Debug.LogWarning("[Bullet] PlayerHealth를 찾지 못했습니다. Player에 PlayerHealth를 추가하세요.");
    }

    private void PlayHitSounds(Vector3 position)
    {
        PlayTemporaryClip(hitClip1, position);
        PlayTemporaryClip(hitClip2, position);
    }

    private void PlayTemporaryClip(AudioClip clip, Vector3 position)
    {
        if (clip == null)
            return;

        GameObject soundObject = new GameObject("BulletHitSound");
        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = hitSoundVolume;
        source.spatialBlend = 0f;
        source.Play();

        Destroy(soundObject, hitSoundDuration);
    }

    private void Deflect()
    {
        deflected = true;
        transform.forward = -transform.forward;
        speed *= deflectSpeedMultiplier;
        Debug.Log("[Bullet] 총알 반사!");
    }

    private void TryDamageEnemy(Collider other)
    {
        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth == null)
            return;

        for (int i = 0; i < deflectedEnemyDamage; i++)
            enemyHealth.Hit();

        Destroy(gameObject);
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
