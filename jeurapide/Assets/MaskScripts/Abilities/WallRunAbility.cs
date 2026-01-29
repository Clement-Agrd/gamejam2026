using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FirstPersonMovement))]
[RequireComponent(typeof(Rigidbody))]
public class WallRunAbility : MonoBehaviour
{
    [Header("Wall Run")]
    public float wallCheckDistance = 0.7f;
    public string wallTag = "WallRunnable";

    [Header("Wall Run Speed")]
    public float wallRunSpeedMultiplier = 1.4f;
    public float maxWallRunSpeed = 12f;

    [Header("Wall Jump")]
    public float wallJumpUpRatio = 1f;
    public float wallJumpSideRatio = 1.2f;
    public float wallJumpForce = 14f;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Air Control")]
    public float airControlLockTime = 1f;

    [Header("Debug")]
    public bool showDebugRay = true;

    FirstPersonMovement player;
    Rigidbody rb;

    Vector3 wallNormal;
    bool isTouchingWall;
    float lockedY;

    void Awake()
    {
        player = GetComponent<FirstPersonMovement>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!player.canWallRun)
            return;

        CheckForWall();

        if (CanWallRun())
            StartWallRun();
        else if (player.IsWallRunning)
            StopWallRun();
    }

    void Update()
    {
        if (player.IsWallRunning && Input.GetKeyDown(jumpKey))
            WallJump();
    }

    // ---------------- WALL CHECK ------------------

    void CheckForWall()
    {
        isTouchingWall = false;
        RaycastHit hit;

        // Droite
        if (Physics.Raycast(transform.position, transform.right, out hit, wallCheckDistance))
        {
            if (hit.collider.CompareTag(wallTag))
                SetWall(hit);
        }
        // Gauche
        else if (Physics.Raycast(transform.position, -transform.right, out hit, wallCheckDistance))
        {
            if (hit.collider.CompareTag(wallTag))
                SetWall(hit);
        }

        if (showDebugRay)
        {
            Debug.DrawRay(transform.position, transform.right * wallCheckDistance, Color.blue);
            Debug.DrawRay(transform.position, -transform.right * wallCheckDistance, Color.red);
        }
    }

    void SetWall(RaycastHit hit)
    {
        isTouchingWall = true;
        wallNormal = hit.normal;
    }

    bool CanWallRun()
    {
        return isTouchingWall
            && !IsGrounded()
            && rb.linearVelocity.y <= 0.1f;
    }

    // ---------------- WALL RUN ------------------

    void StartWallRun()
    {
        if (!player.IsWallRunning)
            lockedY = rb.position.y;

        player.IsWallRunning = true;

        // Verrouille la hauteur
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.MovePosition(new Vector3(rb.position.x, lockedY, rb.position.z));

        // Direction le long du mur
        Vector3 wallForward = Vector3.ProjectOnPlane(transform.forward, wallNormal).normalized;

        // Vitesse actuelle sur le mur
        float currentSpeed = Vector3.Dot(rb.linearVelocity, wallForward);

        // Vitesse cible boostée
        float baseSpeed = player.IsRunning ? player.runSpeed : player.speed;
        float targetSpeed = baseSpeed * wallRunSpeedMultiplier;
        targetSpeed = Mathf.Min(targetSpeed, maxWallRunSpeed);

        // Applique uniquement le delta
        float speedDelta = targetSpeed - currentSpeed;
        if (speedDelta > 0f)
        {
            rb.AddForce(wallForward * speedDelta, ForceMode.VelocityChange);
        }
    }

    void StopWallRun()
    {
        player.IsWallRunning = false;
    }

    // ---------------- WALL JUMP ------------------

    void WallJump()
    {
        player.IsWallRunning = false;

        // Reset velocity
        rb.linearVelocity = Vector3.zero;

        // Direction du jump
        Vector3 jumpDir =
            Vector3.up * wallJumpUpRatio +
            wallNormal * wallJumpSideRatio;

        jumpDir.Normalize();

        // Impulsion
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);

        // Air control recovery
        player.StartAirControlRecovery(airControlLockTime);
    }

    // ---------------- GROUND CHECK ------------------

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
