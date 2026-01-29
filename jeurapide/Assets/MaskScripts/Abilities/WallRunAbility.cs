using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FirstPersonMovement))]
[RequireComponent(typeof(Rigidbody))]
public class WallRunAbility : MonoBehaviour
{
    [Header("Wall Run")]
    public float wallRunForce = 8f;
    public float wallCheckDistance = 0.7f;
    public string wallTag = "WallRunnable";
    float lockedY;

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
        {
            StartWallRun();
        }
        else if (player.IsWallRunning)
        {
            StopWallRun();
        }
    }

    void Update()
    {
        if (player.IsWallRunning && Input.GetKeyDown(jumpKey))
        {
            WallJump();
        }
    }

    // ---------------- WALL CHECK ------------------

    void CheckForWall()
    {
        isTouchingWall = false;
        RaycastHit hit;

        // droite
        if (Physics.Raycast(transform.position, transform.right, out hit, wallCheckDistance))
        {
            if (hit.collider.CompareTag(wallTag))
                SetWall(hit);
        }
        // gauche
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

        // Verrouillage hauteur
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.MovePosition(new Vector3(rb.position.x, lockedY, rb.position.z));

        // Force le long du mur
        Vector3 forwardForce = Vector3.ProjectOnPlane(transform.forward, wallNormal);
        rb.AddForce(forwardForce.normalized * wallRunForce, ForceMode.Acceleration);
    }

    void StopWallRun()
    {
        player.IsWallRunning = false;
    }

    // ---------------- WALL JUMP ------------------

    void WallJump()
    {
        player.IsWallRunning = false;

        // Réinitialise la vélocité
        rb.linearVelocity = Vector3.zero;

        // Détermine la direction du wall jump
        Vector3 jumpDir = Vector3.up * wallJumpUpRatio + (wallNormal) * wallJumpSideRatio;
        jumpDir = jumpDir.normalized;

        // Applique l'impulsion
        rb.AddForce(jumpDir * wallJumpForce, ForceMode.Impulse);

        // Reset air control proprement
        player.StartAirControlRecovery(airControlLockTime);
    }

    // ---------------- IS GROUNDED ------------------

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
