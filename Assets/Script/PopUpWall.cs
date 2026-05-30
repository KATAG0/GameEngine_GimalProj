using UnityEngine;

/// <summary>
/// 벽 프리팹에 붙입니다. 소환 직후 아래에 숨겨 두고, Activate() 시 올라옵니다.
/// </summary>
public class PopUpWall : MonoBehaviour
{
    [SerializeField] private Vector3 hiddenLocalOffset = new Vector3(0f, -3f, 0f);
    [SerializeField] private float riseSpeed = 12f;

    private Vector3 shownPosition;
    private bool isRising;

    private void Awake()
    {
        shownPosition = transform.position;
        transform.position = shownPosition + transform.TransformDirection(hiddenLocalOffset);
    }

    public void Activate()
    {
        isRising = true;
    }

    public void ActivateInstant()
    {
        isRising = false;
        transform.position = shownPosition;
    }

    private void Update()
    {
        if (!isRising)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            shownPosition,
            riseSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, shownPosition) < 0.01f)
        {
            transform.position = shownPosition;
            isRising = false;
        }
    }
}
