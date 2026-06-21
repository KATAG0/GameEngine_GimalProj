using UnityEngine;

/// <summary>
/// YellowOrb(키)를 requiredKeyCount개 모은 뒤에만 Punch로 부술 수 있는 문/벽입니다.
/// 파괴 시 dropItemPrefab(예: DoubleJumpItem)을 드롭합니다.
/// Wall_Middle 같은 오브젝트에 붙이세요.
/// </summary>
public class IfKillEnemyOpenDoor : MonoBehaviour
{
    [Header("Key Requirement")]
    [SerializeField] private int requiredKeyCount = 3;

    [Header("Break")]
    [SerializeField] private int hitPoints = 3;

    [Header("Drop")]
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private Vector3 dropOffset = new Vector3(0f, 0.5f, 0f);

    public void Hit()
    {
        PlayerOrbInventory inventory = FindPlayerInventory();
        if (inventory == null)
        {
            Debug.LogWarning("[IfKillEnemyOpenDoor] Player에 PlayerOrbInventory가 없습니다.");
            return;
        }

        if (inventory.OrbCount < requiredKeyCount)
        {
            Debug.Log($"[IfKillEnemyOpenDoor] 키가 부족합니다. ({inventory.OrbCount} / {requiredKeyCount})");
            return;
        }

        hitPoints--;
        if (hitPoints <= 0)
            BreakApart();
    }

    private void BreakApart()
    {
        SpawnDropItem();
        Destroy(gameObject);
    }

    private void SpawnDropItem()
    {
        if (dropItemPrefab == null)
        {
            Debug.LogWarning("[IfKillEnemyOpenDoor] dropItemPrefab이 할당되지 않았습니다.");
            return;
        }

        Vector3 spawnPos = transform.position + dropOffset;
        Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);
    }

    private static PlayerOrbInventory FindPlayerInventory()
    {
        PlayerOrbInventory[] inventories = Object.FindObjectsOfType<PlayerOrbInventory>();
        if (inventories.Length == 0)
            return null;

        // 씬에 Player 태그가 Tutorial 등에도 붙어 있으면 FindWithTag가 엉뚱한 오브젝트를 잡음
        foreach (PlayerOrbInventory inventory in inventories)
        {
            if (inventory.GetComponent<PlayerController>() != null)
                return inventory;

            if (inventory.GetComponentInParent<PlayerController>() != null)
                return inventory;
        }

        return inventories[0];
    }
}
