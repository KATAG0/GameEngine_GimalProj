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

    private Rigidbody rb;
    private Animator anim;
    private AudioSource audioSource;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

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

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (rotateWithHorizontal && Mathf.Abs(h) > 0.01f)
            transform.Rotate(0f, h * rotationSpeedDeg * Time.deltaTime, 0f, Space.World);

        Vector3 moveXZ = moveAlongFacing
            ? transform.forward * v * moveSpeed
            : new Vector3(h, 0f, v) * moveSpeed;

        rb.velocity = new Vector3(moveXZ.x, rb.velocity.y, moveXZ.z);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            if (audioSource != null && jumpClip != null)
                audioSource.PlayOneShot(jumpClip);
        }

        bool isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        if (anim != null)
            anim.SetBool("Run", isMoving);

        if (Input.GetKeyDown(KeyCode.Mouse1)) // 마우스 우클릭
            anim.SetTrigger("Punch");
    }
}
