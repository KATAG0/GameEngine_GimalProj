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
        Debug.Log($"[PlayerOrbInventory] CollectOrb 호출됨 (수집 전: {orbCount} / {maxOrbCount})");

        if (orbCount >= maxOrbCount)
        {
            Debug.Log($"[PlayerOrbInventory] 이미 최대({maxOrbCount}) → 추가 수집 무시");
            return;
        }

        orbCount++;
        Debug.Log($"[PlayerOrbInventory] ★ 키 수집 성공! 현재: {orbCount} / {maxOrbCount}");
    }
}
