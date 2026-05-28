using UnityEngine;

/// <summary>
/// YellowOrb 수집 개수를 관리합니다. Player 오브젝트에 붙이세요.
/// </summary>
public class PlayerOrbInventory : MonoBehaviour
{
    [SerializeField] private int orbCount;
    [SerializeField] private int maxOrbCount = 3;

    public int OrbCount => orbCount;
    public bool HasMaxOrbs => orbCount >= maxOrbCount;

    public void CollectOrb()
    {
        if (orbCount >= maxOrbCount)
            return;

        orbCount++;
        Debug.Log($"[PlayerOrbInventory] YellowOrb 수집: {orbCount} / {maxOrbCount}");
    }
}
