using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float runSpeed = 9f;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Abilities")]
    public bool canRun = true;
    public bool canJump = true;
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool canGlide = false;
    public bool canFight = true;
    public bool canSeeInvisible = false;
    public bool canPhase = false;

    [Header("Combat Stats")]
    public float damageMultiplier = 1f;
    public float damageReduction = 0f;

    private Rigidbody rb;

    public bool IsRunning { get; private set; }
    public bool IsDashing { get; set; }

    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Capacités par défaut
        canDoubleJump = false;
        canDash = false;
        canGlide = false;
    }

    void FixedUpdate()
    {
        // Bloque le mouvement pendant le dash
        if (IsDashing)
            return;

        IsRunning = canRun && Input.GetKey(runningKey);

        float targetSpeed = IsRunning ? runSpeed : speed;

        if (speedOverrides.Count > 0)
        {
            targetSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 velocity = transform.rotation * new Vector3(
            x * targetSpeed,
            rb.linearVelocity.y,
            z * targetSpeed
        );

        rb.linearVelocity = velocity;
    }

    public Rigidbody Rigidbody => rb;

    public void Stun(float time)
    {
        StartCoroutine(StunCoroutine(time));
    }

    private IEnumerator StunCoroutine(float time)
    {
        canRun = false;
        yield return new WaitForSeconds(time);
        canRun = true;
    }
}