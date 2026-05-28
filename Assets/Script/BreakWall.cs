using UnityEngine;

public class BreakWall : MonoBehaviour
{
    [SerializeField] private int hitPoints = 3;

    public void Hit()
    {
        hitPoints--;
        if (hitPoints <= 0)
            Destroy(gameObject);
    }
}
