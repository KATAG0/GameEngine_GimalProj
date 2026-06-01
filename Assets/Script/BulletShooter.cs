using UnityEngine;

/// <summary>
/// 특정 위치(Fire Point)에서 일정 간격으로 총알을 발사합니다.
/// 총알은 Fire Point가 바라보는 방향(forward)으로 일직선으로 날아갑니다.
/// </summary>
public class BulletShooter : MonoBehaviour
{
    [Header("Bullet")]
    [Tooltip("발사할 총알 프리팹(Bullet 스크립트 포함). 비우면 기본 구체 총알 생성")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 18f;

    [Header("Fire Point")]
    [Tooltip("발사 위치/방향. 비우면 이 오브젝트 자신을 사용")]
    [SerializeField] private Transform firePoint;

    [Header("Fire Timing")]
    [SerializeField] private bool autoFire = true;
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float startDelay = 0f;

    private float timer;

    private void Start()
    {
        timer = startDelay;
    }

    private void Update()
    {
        if (!autoFire)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Fire();
            timer = fireInterval;
        }
    }

    public void Fire()
    {
        Transform origin = firePoint != null ? firePoint : transform;
        Quaternion fireRotation = Quaternion.LookRotation(-origin.forward, origin.up);

        GameObject bulletObj = bulletPrefab != null
            ? Instantiate(bulletPrefab, origin.position, fireRotation)
            : CreateDefaultBullet(origin.position, fireRotation);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
            bullet.SetSpeed(bulletSpeed);
    }

    private static GameObject CreateDefaultBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bulletObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bulletObj.name = "Bullet";
        bulletObj.transform.position = position;
        bulletObj.transform.rotation = rotation;
        bulletObj.transform.localScale = Vector3.one * 0.3f;

        Renderer renderer = bulletObj.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.black;

        SphereCollider col = bulletObj.GetComponent<SphereCollider>();
        if (col != null)
            col.isTrigger = true;

        Rigidbody rb = bulletObj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        bulletObj.AddComponent<Bullet>();
        return bulletObj;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = firePoint != null ? firePoint : transform;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin.position, origin.position - origin.forward * 5f);
        Gizmos.DrawWireSphere(origin.position, 0.2f);
    }
}
