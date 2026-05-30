using System;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    public static event Action OnBroken;
    public static bool HasBeenBroken { get; private set; }

    [SerializeField] private int hitPoints = 3;

    public void Hit()
    {
        hitPoints--;
        if (hitPoints <= 0)
            BreakApart();
    }

    private void BreakApart()
    {
        if (!HasBeenBroken)
        {
            HasBeenBroken = true;
            OnBroken?.Invoke();
        }

        Destroy(gameObject);
    }
}
