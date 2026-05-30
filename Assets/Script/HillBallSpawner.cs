using UnityEngine;

/// <summary>
/// 언덕 위 지정 위치에서 구 프리팹을 주기적으로 소환합니다.
/// Spawn Point의 Y/Z는 고정하고, X만 범위 안에서 랜덤 배치할 수 있습니다.
/// </summary>
public class HillBallSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxAliveBalls = 5;

    [Header("Spawn X Range")]
    [Tooltip("켜면 Spawn Point Y/Z는 유지하고 월드 X만 랜덤")]
    [SerializeField] private bool useRandomSpawnX = true;
    [SerializeField] private float spawnXMin = -28f;
    [SerializeField] private float spawnXMax = -10f;

    [Header("Launch")]
    [Tooltip("소환 직후 가하는 충격력. Spawn Point의 파란 화살표(Z축) 방향")]
    [SerializeField] private float launchForce = 8f;
    [Tooltip("체크 해제 시 월드 Z축 방향으로만 힘 적용")]
    [SerializeField] private bool useSpawnPointLocalZ = true;

    private float spawnTimer;

    private void Update()
    {
        if (ballPrefab == null || spawnPoint == null)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f)
            return;

        if (CountAliveBalls() >= maxAliveBalls)
            return;

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject ball = Instantiate(ballPrefab, spawnPosition, spawnPoint.rotation);

        if (launchForce > 0f)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = useSpawnPointLocalZ
                    ? spawnPoint.forward
                    : Vector3.forward;
                rb.AddForce(direction * launchForce, ForceMode.Impulse);
            }
        }

        spawnTimer = spawnInterval;
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 position = spawnPoint.position;

        if (!useRandomSpawnX)
            return position;

        float minX = Mathf.Min(spawnXMin, spawnXMax);
        float maxX = Mathf.Max(spawnXMin, spawnXMax);
        position.x = Random.Range(minX, maxX);
        return position;
    }

    private int CountAliveBalls()
    {
        RollingBallObstacle[] balls = FindObjectsOfType<RollingBallObstacle>();
        return balls.Length;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null)
            return;

        Vector3 center = spawnPoint.position;

        if (useRandomSpawnX)
        {
            float minX = Mathf.Min(spawnXMin, spawnXMax);
            float maxX = Mathf.Max(spawnXMin, spawnXMax);
            Vector3 left = new Vector3(minX, center.y, center.z);
            Vector3 right = new Vector3(maxX, center.y, center.z);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(left, right);
            Gizmos.DrawWireSphere(left, 0.35f);
            Gizmos.DrawWireSphere(right, 0.35f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(center, 0.5f);
        }

        if (launchForce > 0f)
        {
            Vector3 dir = useSpawnPointLocalZ ? spawnPoint.forward : Vector3.forward;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(center, dir * (launchForce * 0.15f));
        }
    }
}
