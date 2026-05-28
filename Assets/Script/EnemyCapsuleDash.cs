using UnityEngine;

public class EnemyCapsuleDash : MonoBehaviour
{
    private enum State { Idle, Dashing, Cooldown }

    [Header("Detection")]
    [SerializeField] private float detectRange = 8f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 14f;
    [SerializeField] private float dashDuration = 1.2f;
    [SerializeField] private float dashCooldown = 2f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 12f;
    [SerializeField] private float knockbackUpForce = 4f;
    [SerializeField] private float knockbackHitDistance = 1.6f;

    private Transform player;
    private State state = State.Idle;
    private Vector3 dashDirection;
    private float dashTimer;
    private float cooldownTimer;
    private bool knockedBackThisDash;

    private void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (player == null) return;
        switch (state)
        {
            case State.Idle:
                if (GetFlatDistanceToPlayer() <= detectRange) StartDash();
                break;
            case State.Dashing:
                transform.position += dashDirection * dashSpeed * Time.deltaTime;
                dashTimer -= Time.deltaTime;
                TryKnockbackPlayer();
                if (dashTimer <= 0f) EnterCooldown();
                break;
            case State.Cooldown:
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f) state = State.Idle;
                break;
        }
    }

    private void StartDash()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;
        dashDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(dashDirection);
        state = State.Dashing;
        dashTimer = dashDuration;
        knockedBackThisDash = false;
    }

    private void EnterCooldown()
    {
        state = State.Cooldown;
        cooldownTimer = dashCooldown;
    }

    private void TryKnockbackPlayer()
    {
        if (knockedBackThisDash || GetFlatDistanceToPlayer() > knockbackHitDistance) return;
        PlayerKnockback knockback = player.GetComponent<PlayerKnockback>();
        if (knockback == null) knockback = player.GetComponentInParent<PlayerKnockback>();
        if (knockback == null) return;
        knockback.ApplyKnockback(dashDirection, knockbackForce, knockbackUpForce);
        knockedBackThisDash = true;
        EnterCooldown();
    }

    private float GetFlatDistanceToPlayer()
    {
        return Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(player.position.x, 0f, player.position.z));
    }
}
