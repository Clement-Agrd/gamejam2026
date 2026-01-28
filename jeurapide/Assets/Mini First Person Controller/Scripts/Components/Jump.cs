using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FirstPersonMovement))]
public class Jump : MonoBehaviour
{
    Rigidbody rb;
    FirstPersonMovement player;

    [Header("Jump Settings")]
    public float jumpStrength = 2f;

    [SerializeField, Tooltip("Ground check to detect if player is on ground")]
    GroundCheck groundCheck;

    public int jumpCount = 0;   // Compteur de sauts
    public int maxJumps = 1;    // Sauts max autorisés
    private float jumpCooldown = 0.1f;
    private float lastJumpTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<FirstPersonMovement>();

        if (player == null)
            Debug.LogError("[Jump] FirstPersonMovement introuvable !");

        if (groundCheck == null)
            groundCheck = GetComponentInChildren<GroundCheck>();

        // ✅ Assurer que le player commence sans double jump
        player.canDoubleJump = false;
        player.canDash = false;
        player.canGlide = false;
    }


    void Update()
    {
        if (groundCheck != null && groundCheck.isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            jumpCount = 0;
        }

        maxJumps = (player != null && player.canDoubleJump) ? 2 : 1;

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * 100f * jumpStrength);
        jumpCount++;
        lastJumpTime = Time.time; // ← on met à jour le cooldown
    }
}