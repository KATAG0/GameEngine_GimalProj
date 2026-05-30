using UnityEngine;

/// <summary>
/// 플레이어가 밟으면 벽 프리팹을 소환합니다.
/// 바닥 블록: Box Collider (Is Trigger) + 이 스크립트.
/// </summary>
public class PopUpWallTrigger : MonoBehaviour
{
    [Header("Wall Prefab")]
    [SerializeField] private GameObject wallPrefab;
    [Tooltip("벽이 막힐 최종 위치. 비우면 이 오브젝트 위치 + Spawn Offset 사용")]
    [SerializeField] private Transform wallSpawnPoint;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0f, 2f);

    [Header("Spawn Behavior")]
    [Tooltip("체크 시 아래에서 올라옴. 해제 시 즉시 최종 위치에 나타남")]
    [SerializeField] private bool riseFromBelow = true;
    [SerializeField] private bool hideTriggerMeshAfterUse = true;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used || !other.CompareTag("Player"))
            return;

        if (wallPrefab != null)
            SpawnWall();

        used = true;

        if (hideTriggerMeshAfterUse)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = false;
        }
    }

    private void SpawnWall()
    {
        Vector3 position = wallSpawnPoint != null
            ? wallSpawnPoint.position
            : transform.position + transform.TransformDirection(spawnOffset);

        Quaternion rotation = wallSpawnPoint != null
            ? wallSpawnPoint.rotation
            : transform.rotation;

        GameObject wall = Instantiate(wallPrefab, position, rotation);

        PopUpWall popUpWall = wall.GetComponent<PopUpWall>();
        if (popUpWall == null)
            return;

        if (riseFromBelow)
            popUpWall.Activate();
        else
            popUpWall.ActivateInstant();
    }

    private void OnDrawGizmosSelected()
    {
        if (wallSpawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(wallSpawnPoint.position, new Vector3(1f, 2f, 0.3f));
            return;
        }

        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + transform.TransformDirection(spawnOffset);
        Gizmos.DrawWireCube(pos, new Vector3(1f, 2f, 0.3f));
    }
}
