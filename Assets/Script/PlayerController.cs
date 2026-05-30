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

    private Rigidbody rb;
    private Animator anim;
    private AudioSource audioSource;
    private PlayerKnockback playerKnockback;
    private bool isGrounded;
    private int airJumpsRemaining;
    private bool hasDoubleJumpUnlocked;

    public bool HasDoubleJump => hasDoubleJumpUnlocked;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerKnockback = GetComponent<PlayerKnockback>();

        if (anim == null)
            Debug.LogError("Animator? ?? ? ??!");
    }

    private void Update()
    {
        if (anim != null)
            anim.SetBool("Jump", !isGrounded);

        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            groundCheckDist);

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

        if (Input.GetKeyDown(KeyCode.Mouse1)) // 마우스 우클릭
        {
            if (anim != null)
                anim.SetTrigger("Punch");
            TryPunch();
        }
    }

    public void UnlockDoubleJump()
    {
        hasDoubleJumpUnlocked = true;

        if (isGrounded)
            airJumpsRemaining = 1;
    }

    private void TryJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

        if (audioSource != null && jumpClip != null)
            audioSource.PlayOneShot(jumpClip);
    }

    private void TryPunch()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;

        if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, punchDistance))
            return;

        IfKillEnemyOpenDoor gatedDoor = hit.collider.GetComponent<IfKillEnemyOpenDoor>();
        if (gatedDoor != null)
        {
            gatedDoor.Hit();
            return;
        }

        BreakWall breakWall = hit.collider.GetComponent<BreakWall>();
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
