using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FirstPersonMovement))]
public class DashAbility : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 8f;
    public float dashSpeed = 50f;
    public float dashCooldown = 1f;
    public float damage = 25f;

    [Header("References")]
    public Transform cameraTransform;
    [SerializeField] GroundCheck groundCheck;

    [Header("Physics")]
    public bool disableGravityDuringDash = true;
    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    [Header("Air Dash")]
    public int maxAirDash = 1;
    private int dashCount = 0;

    [Header("Audio")]
    public AudioSource dashAudio;

    private FirstPersonMovement player;
    private bool canDashNow = true;

    void Awake()
    {
        player = GetComponent<FirstPersonMovement>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (groundCheck == null)
            groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void OnEnable()
    {
        if (groundCheck != null)
            groundCheck.Grounded += OnGrounded;
    }

    void OnDisable()
    {
        if (groundCheck != null)
            groundCheck.Grounded -= OnGrounded;
    }

    void Update()
    {
        if (!player.canDash || !canDashNow)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!IsGrounded() && dashCount >= maxAirDash)
                return;

            StartCoroutine(Dash());
        }
    }

    bool IsGrounded()
    {
        return groundCheck != null && groundCheck.isGrounded;
    }

    private IEnumerator Dash()
    {
        canDashNow = false;
        player.IsDashing = true;

        // 🔊 SON DE DASH
        if (dashAudio != null)
            dashAudio.Play();

        if (!IsGrounded())
            dashCount++;

        Rigidbody rb = player.Rigidbody;

        if (disableGravityDuringDash)
            rb.useGravity = false;

        Vector3 dashDir = cameraTransform.forward.normalized;
        float remainingDistance = dashDistance;

        while (remainingDistance > 0f)
        {
            float step = dashSpeed * Time.deltaTime;

            if (rb.SweepTest(dashDir, out RaycastHit hitObstacle, step, QueryTriggerInteraction.Ignore))
                break;

            if (Physics.Raycast(rb.position, dashDir, out RaycastHit hitEnemy, step, enemyLayer))
            {
                Health health = hitEnemy.collider.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(damage);
            }

            rb.linearVelocity = dashDir * dashSpeed;
            remainingDistance -= step;

            yield return null;
        }

        if (disableGravityDuringDash)
            rb.useGravity = true;

        player.IsDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDashNow = true;
    }

    void OnGrounded()
    {
        dashCount = 0;
    }
}
