using UnityEngine;

/// <summary>
/// 자식 발판들을 관리합니다. 부모 빈 오브젝트(그룹)에 붙이세요.
/// </summary>
public class SteppingPlatformGroup : MonoBehaviour
{
    public enum StepMode
    {
        [Tooltip("숨겨진 발판 중 가장 가까운 것의 MeshRenderer 켜기")]
        RevealClosestHidden,
        [Tooltip("보이는 발판 중 가장 가까운 것의 MeshRenderer 끄기")]
        HideClosestVisible
    }

    [SerializeField] private StepMode stepMode = StepMode.RevealClosestHidden;
    [SerializeField] private SteppingPlatform[] platforms;

    private void Awake()
    {
        if (platforms == null || platforms.Length == 0)
            platforms = GetComponentsInChildren<SteppingPlatform>();
    }

    public void OnPlayerStepped(SteppingPlatform stepped, Transform player)
    {
        if (platforms == null || platforms.Length == 0)
            return;

        MeshRenderer steppedRenderer = stepped.GetComponent<MeshRenderer>();
        if (steppedRenderer == null || !steppedRenderer.enabled)
            return;

        SteppingPlatform closest = null;
        float minDistance = float.MaxValue;

        foreach (SteppingPlatform platform in platforms)
        {
            if (platform == null || platform == stepped)
                continue;

            MeshRenderer renderer = platform.GetComponent<MeshRenderer>();
            if (renderer == null)
                continue;

            bool isHidden = !renderer.enabled;

            if (stepMode == StepMode.RevealClosestHidden && !isHidden)
                continue;

            if (stepMode == StepMode.HideClosestVisible && isHidden)
                continue;

            float distance = Vector3.Distance(
                player.position,
                platform.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = platform;
            }
        }

        if (closest == null)
            return;

        MeshRenderer closestRenderer = closest.GetComponent<MeshRenderer>();
        if (closestRenderer == null)
            return;

        closestRenderer.enabled = stepMode == StepMode.RevealClosestHidden;
    }
}
