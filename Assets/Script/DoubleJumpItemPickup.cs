using UnityEngine;

/// <summary>
/// 플레이어가 닿으면 더블점프를 해금합니다.
/// DoubleJumpItem에 붙이고 Collider의 Is Trigger를 켜 두세요.
/// </summary>
public class DoubleJumpItemPickup : MonoBehaviour
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

        controller.UnlockDoubleJump();
        Destroy(gameObject);
    }
}
