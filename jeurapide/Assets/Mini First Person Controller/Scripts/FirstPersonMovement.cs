using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Ability")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;
    public bool canJump = true;
    public bool canDoubleJump;
    public bool canDash;
    public bool canGlide;
    public bool canFight = true;
    public bool canSeeInvisible;
    public bool canPhase;

    public float damageMultiplier = 1f;
    public float damageReduction = 0f;

    public void Stun(float time)
    {
        StartCoroutine(StunCoroutine(time));
    }

    private IEnumerator StunCoroutine(float time)
    {
        canRun = false; // ou disable input / déplacement
        yield return new WaitForSeconds(time);
        canRun = true;
    }

    Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();



    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    
        // Forcer les capacités de base
        canDoubleJump = false;
        canDash = false;
        canGlide = false;
    }
    void Start()
    {
        canDoubleJump = false;
        canDash = false;
        canGlide = false;
    }
    

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.linearVelocity.y, targetVelocity.y);
        
    }
}