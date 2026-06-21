using UnityEngine;

/// <summary>
/// 플레이어가 닿으면 수집되고 사라집니다.
/// Sphere Collider의 Is Trigger를 켜 두세요.
/// </summary>
public class YellowOrb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[YellowOrb] Trigger 진입: {other.name} (tag={other.tag})");

        if (!IsPlayer(other))
        {
            Debug.Log($"[YellowOrb] Player 태그 아님 → 수집 안 함");
            return;
        }

        PlayerOrbInventory inventory = other.GetComponentInParent<PlayerOrbInventory>();
        if (inventory == null)
            inventory = other.transform.root.GetComponentInChildren<PlayerOrbInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("[YellowOrb] Player에 PlayerOrbInventory 없음 → 키는 사라지지만 개수는 안 올라감");
            Destroy(gameObject);
            return;
        }

        Debug.Log($"[YellowOrb] 키 수집 시도 (현재 보유: {inventory.OrbCount})");
        inventory.CollectOrb();
        Debug.Log($"[YellowOrb] 수집 완료 후 보유: {inventory.OrbCount}");

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
