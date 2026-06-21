using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [Tooltip("?/? ?? ? Y??? ?? ?? (?? ??? ?? 360�?? ?? ??)")]
    [SerializeField] private bool rotateWithHorizontal = true;
    [SerializeField] private float rotationSpeedDeg = 180f;
    [Tooltip("?? W/S? ??�??(???? ?? ??). ?? ???? ?? XZ? ??")]
    [SerializeField] private bool moveAlongFacing = true;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDist = 0.4f;

    [Header("Jump SFX")]
    [SerializeField] private AudioClip jumpClip;

    [Header("Punch")]
    [SerializeField] private float punchDistance = 2f;
    [SerializeField] private float punchHitDelay = 0.2f;
    [SerializeField] private string punchAnimStateName = "Punching";

    [Header("Punch SFX")]
    [SerializeField] private AudioClip punchClip1;
    [SerializeField] private AudioClip punchClip2;

    private Rigidbody rb;
    private Animator anim;
    private AudioSource audioSource;
    private PlayerKnockback playerKnockback;
    private bool isGrounded;
    private int airJumpsRemaining;
    private bool hasDoubleJumpUnlocked;
    private bool isPunching;
    private bool usePunchClip1 = true;
    private bool hasBulletDeflectUnlocked;

    public bool HasDoubleJump => hasDoubleJumpUnlocked;
    public bool HasBulletDeflect => hasBulletDeflectUnlocked;
    public bool IsDeflecting => isPunching && hasBulletDeflectUnlocked;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = GetComponentInChildren<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
        playerKnockback = GetComponent<PlayerKnockback>();

        if (anim == null)
            Debug.LogError("Animator? ?? ? ??!");
    }

    private void Update()
    {
        if (anim != null)
            anim.SetBool("Jump", !isGrounded);

        isGrounded = IsGrounded();

        if (isGrounded)
            airJumpsRemaining = hasDoubleJumpUnlocked ? 1 : 0;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (rotateWithHorizontal && Mathf.Abs(h) > 0.01f)
            transform.Rotate(0f, h * rotationSpeedDeg * Time.deltaTime, 0f, Space.World);

        Vector3 moveXZ = moveAlongFacing
            ? transform.forward * v * moveSpeed
            : new Vector3(h, 0f, v) * moveSpeed;

        if (playerKnockback == null || !playerKnockback.IsKnockedBack)
            rb.velocity = new Vector3(moveXZ.x, rb.velocity.y, moveXZ.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
                TryJump();
            else if (airJumpsRemaining > 0)
            {
                airJumpsRemaining--;
                TryJump();
            }
        }

        bool isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        if (anim != null)
            anim.SetBool("Run", isMoving);

        if (Input.GetKeyDown(KeyCode.Mouse0)) // 마우스 좌클릭
        {
            if (isPunching || !isGrounded)
                return;

            if (anim != null)
                anim.SetTrigger("Punch");

            StartCoroutine(PunchHitRoutine());
        }
    }

    public void UnlockDoubleJump()
    {
        hasDoubleJumpUnlocked = true;

        if (isGrounded)
            airJumpsRemaining = 1;
    }

    public void UnlockBulletDeflect()
    {
        hasBulletDeflectUnlocked = true;
        Debug.Log("[PlayerController] 총알 반사 능력이 해금되었습니다!");
    }

    private void TryJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

        if (audioSource != null && jumpClip != null)
            audioSource.PlayOneShot(jumpClip);
    }

    private void PlayPunchSfx()
    {
        AudioClip clip = usePunchClip1 ? punchClip1 : punchClip2;
        usePunchClip1 = !usePunchClip1;

        if (clip == null)
        {
            Debug.LogWarning("[PlayerController] Punch Clip이 할당되지 않았습니다.");
            return;
        }

        if (audioSource != null)
            audioSource.PlayOneShot(clip);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    private IEnumerator PunchHitRoutine()
    {
        isPunching = true;
        yield return new WaitForSeconds(punchHitDelay);

        if (IsGrounded())
        {
            PlayPunchSfx();
            TryPunch();
        }

        yield return WaitUntilPunchAnimationFinished();
        isPunching = false;
    }

    private IEnumerator WaitUntilPunchAnimationFinished()
    {
        if (anim == null)
        {
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        int punchHash = Animator.StringToHash(punchAnimStateName);
        const float enterTimeout = 0.5f;
        float elapsed = 0f;

        while (!IsInPunchAnimation(punchHash) && elapsed < enterTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!IsInPunchAnimation(punchHash))
            yield break;

        while (IsInPunchAnimation(punchHash))
            yield return null;
    }

    private bool IsInPunchAnimation(int punchHash)
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        return state.shortNameHash == punchHash;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDist);
    }

    private void TryPunch()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;

        if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, punchDistance))
            return;

        IfKillEnemyOpenDoor gatedDoor = hit.collider.GetComponent<IfKillEnemyOpenDoor>();
        if (gatedDoor == null)
            gatedDoor = hit.collider.GetComponentInParent<IfKillEnemyOpenDoor>();
        if (gatedDoor != null)
        {
            gatedDoor.Hit();
            return;
        }

        BreakWall breakWall = hit.collider.GetComponent<BreakWall>();
        if (breakWall == null)
            breakWall = hit.collider.GetComponentInParent<BreakWall>();
        if (breakWall != null)
        {
            breakWall.Hit();
            return;
        }

        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
        if (enemyHealth == null)
            enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
            enemyHealth.Hit();
    }
}
