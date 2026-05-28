using UnityEngine;

/// <summary>
/// Punch로 hitPoints만큼 맞으면 Enemy가 파괴되고 YellowOrb를 드롭합니다.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int hitPoints = 3;

    [Header("Drop")]
    [SerializeField] private GameObject yellowOrbPrefab;
    [SerializeField] private Vector3 dropOffset = new Vector3(0f, 0.5f, 0f);

    public void Hit()
    {
        if (hitPoints <= 0)
            return;

        hitPoints--;
        if (hitPoints <= 0)
            Die();
    }

    private void Die()
    {
        SpawnYellowOrb();
        Destroy(gameObject);
    }

    private void SpawnYellowOrb()
    {
        Vector3 spawnPos = transform.position + dropOffset;

        if (yellowOrbPrefab != null)
        {
            Instantiate(yellowOrbPrefab, spawnPos, Quaternion.identity);
            return;
        }

        CreateDefaultYellowOrb(spawnPos);
    }

    private static void CreateDefaultYellowOrb(Vector3 position)
    {
        GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        orb.name = "YellowOrb";
        orb.transform.position = position;
        orb.transform.localScale = Vector3.one * 0.6f;

        Renderer renderer = orb.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.yellow;

        Object.Destroy(orb.GetComponent<Collider>());

        SphereCollider trigger = orb.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 0.5f;

        orb.AddComponent<YellowOrb>();
    }
}
