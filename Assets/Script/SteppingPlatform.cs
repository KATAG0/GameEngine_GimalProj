using UnityEngine;

/// <summary>
/// 그룹 안의 발판 하나. MeshRenderer가 켜진 블록을 밟으면 그룹에 알립니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SteppingPlatform : MonoBehaviour
{
    [SerializeField] private SteppingPlatformGroup group;
    [SerializeField] private float stepCooldown = 0.25f;

    private float lastStepTime = -999f;

    private void Awake()
    {
        if (group == null)
            group = GetComponentInParent<SteppingPlatformGroup>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        if (Time.time - lastStepTime < stepCooldown)
            return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null || !renderer.enabled)
            return;

        if (group == null)
        {
            Debug.LogWarning($"[SteppingPlatform] {name}: SteppingPlatformGroup이 없습니다.");
            return;
        }

        lastStepTime = Time.time;
        group.OnPlayerStepped(this, collision.transform);
    }
}
