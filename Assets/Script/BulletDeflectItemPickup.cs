using UnityEngine;

/// <summary>
/// 플레이어가 닿으면 '총알 반사' 능력을 해금합니다.
/// 이후 총알이 날아올 때 펀치(공격)하면 총알이 튕겨 나갑니다.
/// 아이템 오브젝트에 붙이고 Collider의 Is Trigger를 켜 두세요.
/// </summary>
public class BulletDeflectItemPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller == null)
            controller = other.GetComponentInParent<PlayerController>();

        if (controller == null)
            return;

        controller.UnlockBulletDeflect();
        Destroy(gameObject);
    }
}
