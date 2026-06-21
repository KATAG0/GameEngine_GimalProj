using UnityEngine;

/// <summary>
/// 총알 제거 블록. Bullet이 이 블록에 닿으면 총알을 삭제합니다.
/// 블록 오브젝트에 Collider(Is Trigger)와 함께 붙이세요.
/// </summary>
public class BulletDestroyBlock : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet == null)
            bullet = other.GetComponentInParent<Bullet>();

        if (bullet != null)
            Destroy(bullet.gameObject);
    }
}
