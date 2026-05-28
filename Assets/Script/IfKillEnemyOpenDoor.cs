using UnityEngine;

/// <summary>
/// YellowOrb(키)를 requiredKeyCount개 모은 뒤에만 Punch로 부술 수 있는 문/벽입니다.
/// Wall_Middle 같은 오브젝트에 붙이세요.
/// </summary>
public class IfKillEnemyOpenDoor : MonoBehaviour
{
    [Header("Key Requirement")]
    [SerializeField] private int requiredKeyCount = 3;

    [Header("Break")]
    [SerializeField] private int hitPoints = 3;

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
            Destroy(gameObject);
    }

    private static PlayerOrbInventory FindPlayerInventory()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
            return null;

        PlayerOrbInventory inventory = player.GetComponent<PlayerOrbInventory>();
        if (inventory == null)
            inventory = player.GetComponentInParent<PlayerOrbInventory>();

        return inventory;
    }
}
