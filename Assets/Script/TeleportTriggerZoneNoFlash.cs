using System.Collections;
using UnityEngine;

/// <summary>
/// 보이지 않는 트리거 박스. Player가 들어오면 delaySeconds 후 목적지로 순간이동합니다.
/// TeleportTriggerZone과 동일하지만, 순간이동 후 화면 플래시(눈뽕) 효과가 없습니다.
/// Box Collider (Is Trigger) + 이 스크립트.
/// </summary>
public class TeleportTriggerZoneNoFlash : MonoBehaviour
{
    [Header("Teleport")]
    [Tooltip("이동할 위치(Transform). 비우면 아래 Teleport Position 좌표를 사용")]
    [SerializeField] private Transform teleportDestination;
    [Tooltip("직접 입력하는 목적지 좌표. Teleport Destination이 비어 있을 때 사용")]
    [SerializeField] private Vector3 teleportPosition;
    [Tooltip("닿은 뒤 이동까지의 지연(초). 0이면 즉시 이동")]
    [SerializeField] private float delaySeconds = 0f;

    [Header("Behavior")]
    [Tooltip("체크 시 1회만 발동. 해제하면 닿을 때마다 발동")]
    [SerializeField] private bool triggerOnce = false;
    [Tooltip("체크 시 목적지의 회전도 맞춤")]
    [SerializeField] private bool matchDestinationRotation;

    [Header("Damage")]
    [Tooltip("순간이동 시 플레이어가 받는 데미지(0이면 데미지 없음)")]
    [SerializeField] private int teleportDamage = 1;

    private bool used;
    private Coroutine teleportRoutine;
    private Transform pendingPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other))
            return;

        if (triggerOnce && used)
            return;

        Transform player = GetPlayerTransform(other);
        if (player == null)
            return;

        if (teleportRoutine != null)
            StopCoroutine(teleportRoutine);

        pendingPlayer = player;
        teleportRoutine = StartCoroutine(TeleportAfterDelay());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other))
            return;

        if (teleportRoutine == null)
            return;

        StopCoroutine(teleportRoutine);
        teleportRoutine = null;
        pendingPlayer = null;
    }

    private IEnumerator TeleportAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (pendingPlayer != null)
            TeleportPlayer(pendingPlayer);

        teleportRoutine = null;
        pendingPlayer = null;

        if (triggerOnce)
            used = true;
    }

    private void TeleportPlayer(Transform player)
    {
        Vector3 destination = teleportDestination != null
            ? teleportDestination.position
            : teleportPosition;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = destination;
        }
        else
        {
            player.position = destination;
        }

        if (matchDestinationRotation && teleportDestination != null)
            player.rotation = teleportDestination.rotation;

        ApplyTeleportDamage(player);
    }

    private void ApplyTeleportDamage(Transform player)
    {
        if (teleportDamage <= 0)
            return;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health == null)
            health = player.GetComponentInParent<PlayerHealth>();

        if (health != null)
            health.TakeDamage(teleportDamage);
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

    private static Transform GetPlayerTransform(Collider other)
    {
        if (other.CompareTag("Player"))
            return other.transform;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.CompareTag("Player"))
            return rb.transform;

        Transform current = other.transform;
        while (current != null)
        {
            if (current.CompareTag("Player"))
                return current;
            current = current.parent;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 destination = teleportDestination != null
            ? teleportDestination.position
            : teleportPosition;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(destination, 0.5f);
        Gizmos.DrawLine(transform.position, destination);
    }
}
