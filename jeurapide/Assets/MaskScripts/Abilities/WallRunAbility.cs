using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FirstPersonMovement))]
[RequireComponent(typeof(Rigidbody))]
public class WallRunAbility : MonoBehaviour
{
    [Header("Wall Run")]
    public float wallCheckDistance = 0.9f;
    public string wallTag = "WallRunnable";

    [Header("Wall Run Conditions")]
    public float minWallRunSpeed = 6f;

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

        // Angles pour la détection en diagonale
        float[] angles = { -45f, -25f, 0f, 25f, 45f };

        // Côté droit
        foreach (float angle in angles)
        {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * transform.right;

            if (Physics.Raycast(transform.position, dir, out hit, wallCheckDistance))
            {
                if (hit.collider.CompareTag(wallTag))
                {
                    SetWall(hit);
                    break;
                }
            }

            if (showDebugRay)
                Debug.DrawRay(transform.position, dir * wallCheckDistance, Color.blue);
        }

        // Côté gauche
        foreach (float angle in angles)
        {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * -transform.right;

            if (Physics.Raycast(transform.position, dir, out hit, wallCheckDistance))
            {
                if (hit.collider.CompareTag(wallTag))
                {
                    SetWall(hit);
                    break;
                }
            }

            if (showDebugRay)
                Debug.DrawRay(transform.position, dir * wallCheckDistance, Color.red);
        }
    }

    void SetWall(RaycastHit hit)
    {
        isTouchingWall = true;
        wallNormal = hit.normal;
    }

    bool CanWallRun()
    {
        float horizontalSpeed = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        ).magnitude;

        return isTouchingWall
            && !IsGrounded()
            && rb.linearVelocity.y <= 0.1f
            && horizontalSpeed >= minWallRunSpeed;
    }

    // ---------------- WALL RUN ------------------

    void StartWallRun()
    {
        // Première frame du wall run
        if (!player.IsWallRunning)
        {
            lockedY = rb.position.y;
            player.IsWallRunning = true;
        }

        // Direction le long du mur
        Vector3 wallForward = Vector3.ProjectOnPlane(transform.forward, wallNormal).normalized;

        float v = Input.GetAxis("Vertical");
        if (v <= 0f)
            return;

        // Bonus uniquement si le joueur court
        bool isRunning = player.IsRunning;
        float speedMultiplier = isRunning ? wallRunSpeedMultiplier : 1f;

        Vector3 desiredVelocity = wallForward * v * player.speed * speedMultiplier;
        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxWallRunSpeed);

        // Applique la vitesse sans auto-avance
        rb.AddForce(desiredVelocity - rb.linearVelocity, ForceMode.Acceleration);

        // Verrouillage vertical SEULEMENT si le joueur court
        if (isRunning)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.MovePosition(new Vector3(rb.position.x, lockedY, rb.position.z));
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

        rb.linearVelocity = Vector3.zero;

        Vector3 jumpDir =
            Vector3.up * wallJumpUpRatio +
            wallNormal * wallJumpSideRatio;

        jumpDir.Normalize();

        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);

        player.StartAirControlRecovery(airControlLockTime);
    }

    // ---------------- GROUND CHECK ------------------

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
