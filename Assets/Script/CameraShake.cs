using UnityEngine;

/// <summary>
/// 카메라 시야를 흔드는 효과(회전 기반). 실제로 렌더링하는 Camera 오브젝트에 붙이세요.
/// CameraMovement는 자식 카메라의 위치만 제어하므로 회전 흔들림과 충돌하지 않습니다.
/// 다른 스크립트에서 CameraShake.Instance.Shake() 로 호출합니다.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Default Shake")]
    [Tooltip("흔들림 지속 시간(초)")]
    [SerializeField] private float defaultDuration = 0.4f;
    [Tooltip("흔들림 세기(각도)")]
    [SerializeField] private float defaultMagnitude = 6f;

    private Quaternion originalLocalRotation;
    private float shakeTimeRemaining;
    private float totalDuration;
    private float magnitude;

    private void Awake()
    {
        Instance = this;
        originalLocalRotation = transform.localRotation;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float damper = totalDuration > 0f ? Mathf.Clamp01(shakeTimeRemaining / totalDuration) : 0f;
            float angle = magnitude * damper;

            Vector3 offset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)) * angle;

            transform.localRotation = originalLocalRotation * Quaternion.Euler(offset);
        }
        else
        {
            transform.localRotation = originalLocalRotation;
        }
    }

    public void Shake()
    {
        Shake(defaultDuration, defaultMagnitude);
    }

    public void Shake(float duration, float shakeMagnitude)
    {
        // 더 강한 흔들림이 들어오면 갱신, 약하면 남은 흔들림 유지
        if (shakeMagnitude >= magnitude || shakeTimeRemaining <= 0f)
            magnitude = shakeMagnitude;

        totalDuration = duration;
        shakeTimeRemaining = duration;
    }
}
