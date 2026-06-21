using UnityEngine;

/// <summary>
/// Player가 특정 블록/구역에 닿으면 BGM을 변경합니다.
/// 블록 오브젝트에 Box Collider(Is Trigger)와 함께 붙이세요.
/// </summary>
public class BgmChangeTrigger : MonoBehaviour
{
    [Header("BGM")]
    [Tooltip("현재 BGM을 재생 중인 AudioSource")]
    [SerializeField] private AudioSource bgmSource;
    [Tooltip("이 블록에 닿았을 때 바꿀 BGM")]
    [SerializeField] private AudioClip nextBgm;

    [Header("Behavior")]
    [Tooltip("체크 시 한 번만 BGM 변경")]
    [SerializeField] private bool triggerOnce = true;
    [Tooltip("BGM 변경 후 반복 재생")]
    [SerializeField] private bool loop = true;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnce && used)
            return;

        if (!IsPlayer(other))
            return;

        ChangeBgm();
    }

    private void ChangeBgm()
    {
        if (bgmSource == null || nextBgm == null)
        {
            Debug.LogWarning("[BgmChangeTrigger] BGM Source 또는 Next BGM이 연결되지 않았습니다.");
            return;
        }

        if (bgmSource.clip == nextBgm && bgmSource.isPlaying)
        {
            used = true;
            return;
        }

        bgmSource.clip = nextBgm;
        bgmSource.loop = loop;
        bgmSource.Play();
        used = true;

        Debug.Log("[BgmChangeTrigger] BGM 변경 완료");
    }

    private static bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player"))
            return true;

        Transform current = other.transform.parent;
        while (current != null)
        {
            if (current.CompareTag("Player"))
                return true;
            current = current.parent;
        }

        return false;
    }
}
