using UnityEngine;

/// <summary>
/// 플레이어가 닿으면 수집되고 사라집니다.
/// Sphere Collider의 Is Trigger를 켜 두세요.
/// </summary>
public class YellowOrb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerOrbInventory inventory = other.GetComponent<PlayerOrbInventory>();
        if (inventory == null)
            inventory = other.GetComponentInParent<PlayerOrbInventory>();

        if (inventory != null)
            inventory.CollectOrb();

        Destroy(gameObject);
    }
}
