using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FirstPersonMovement))]
public class GlideAbility : MonoBehaviour
{
    [Header("Input")]
    public KeyCode glideKey = KeyCode.Space;
    

    [Header("Glide Settings")]
    [Range(0f, 1f)]
    public float glideGravityMultiplier = 0.3f;
    public float maxFallSpeed = -2.5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 1.3f;

    private Rigidbody rb;
    private FirstPersonMovement player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<FirstPersonMovement>();

       

        if (rb == null)
            Debug.LogError("[GlideAbility] Rigidbody MANQUANT");

        if (player == null)
            Debug.LogError("[GlideAbility] FirstPersonMovement MANQUANT");
    }

    void FixedUpdate()
    {

        if (player == null)
        {
            return;
        }

        if (!player.canGlide)
        {
            return;
        }

        if (!Input.GetKey(glideKey))
        {
            return;
        }

        if (IsGrounded())
        {
            return;
        }
        ApplyGlide();
    }

    private void ApplyGlide()
    {
        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < maxFallSpeed)
        {
            velocity.y = maxFallSpeed;
        }

        rb.linearVelocity = velocity;

        Vector3 reducedGravity = Physics.gravity * glideGravityMultiplier;
        rb.AddForce(-reducedGravity * rb.mass, ForceMode.Force);
        
    }

    private bool IsGrounded()
    {
        bool grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance
        );
        
        return grounded;
    }
}