using UnityEngine;

/// <summary>
/// X축 구간을 왕복 이동합니다. Box Collider(Is Trigger 해제)를 붙이면 Player 이동을 막을 수 있습니다.
/// </summary>
public class PingPongX : MonoBehaviour
{
    [SerializeField] private float minX = -69.88f;
    [SerializeField] private float maxX = -57.94f;
    [SerializeField] private float moveSpeed = 3f;

    private float targetX;

    private void Start()
    {
        float left = Mathf.Min(minX, maxX);
        float right = Mathf.Max(minX, maxX);
        minX = left;
        maxX = right;

        targetX = transform.position.x <= (minX + maxX) * 0.5f ? maxX : minX;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, moveSpeed * Time.deltaTime);
        transform.position = pos;

        if (Mathf.Abs(pos.x - targetX) < 0.01f)
            targetX = targetX == maxX ? minX : maxX;
    }

    private void OnDrawGizmosSelected()
    {
        float left = Mathf.Min(minX, maxX);
        float right = Mathf.Max(minX, maxX);
        Vector3 center = transform.position;
        Vector3 a = new Vector3(left, center.y, center.z);
        Vector3 b = new Vector3(right, center.y, center.z);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawWireSphere(a, 0.25f);
        Gizmos.DrawWireSphere(b, 0.25f);
    }
}
