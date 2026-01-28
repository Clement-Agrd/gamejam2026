using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FirstPersonMovement))]
public class Jump : MonoBehaviour
{
    Rigidbody rb;
    FirstPersonMovement player;

    [Header("Jump Settings")]
    public float jumpForce = 5f;

    [SerializeField]
    GroundCheck groundCheck;

    public int jumpCount = 0;
    public int maxJumps = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<FirstPersonMovement>();

        if (groundCheck == null)
            groundCheck = GetComponentInChildren<GroundCheck>();

        // État initial sans capacités
        player.canDoubleJump = false;
        player.canDash = false;
        player.canGlide = false;
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
        maxJumps = player.canDoubleJump ? 2 : 1;

        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            PerformJump();
        }
    }
    IEnumerator ConfirmJump()
    {
        yield return new WaitForFixedUpdate();

        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            jumpCount++;
        }
    }


    void PerformJump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(ConfirmJump());
    }


    void OnGrounded()
    {
        jumpCount = 0;
    }
}