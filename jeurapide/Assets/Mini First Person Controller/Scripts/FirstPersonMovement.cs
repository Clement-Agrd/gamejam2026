using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField]
    GroundCheck groundCheck;
    
    [Header("Advanced Movement Feel")]
    public float groundAcceleration = 40f;
    public float airAcceleration = 15f;
    public float maxAirSpeed = 12f;

    [Header("Better Gravity")]
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;

    [Header("Movement")]
    public float speed = 7f;
    public float runSpeed = 12f;
    
    [Header("Air Control")]
    public float airControlMultiplier = 0.5f;
    public float airControlRecoveryTime = 1f;
    public float currentAirControl;

    [Header("Abilities")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public KeyCode runningKey = KeyCode.LeftShift;

    public bool canJump = true;
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool canGlide = false;
    public bool canFight = true;
    public bool canSeeInvisible = false;
    public bool canPassThruWall = false;
    public bool canWallRun;
    public bool IsWallRunning { get; set; }
    public bool canBreakShield = false;

    [Header("Combat Stats")]
    public float damageMultiplier = 1f;
    public float damageReduction = 0f;

    [Header("Physics")]
    public float gravityMultiplier = 1f;

    [Header("Falling Impact")]
    public bool enableFallingImpact = false; // activé uniquement par Oni
    public float fallImpactTime = 1f;       
    public float fallImpactRadius = 3f;     
    public float fallImpactDamage = 50f; 


    [Header("Combat / Massue Oni")]
    public bool canAttack = true;
    public float attackRange = 2f;
    public float attackDamage = 20f;
    public LayerMask enemyLayer;
    public Animator animator;            

    public GameObject massue;             // Massue visible uniquement Oni
    public AnimationClip attackClip1;     
    public AnimationClip attackClip2;     

    [HideInInspector]
    public bool isGrounded = true;

    public float fallTimer = 0f;
    private Rigidbody rb;

    public bool IsDashing { get; set; }
    /// <summary>
    /// Functions to override movement speed (last added wins)
    /// </summary>

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (groundCheck == null)
            groundCheck = GetComponentInChildren<GroundCheck>();

        // Paramètres physiques conseillés
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        currentAirControl = airControlMultiplier;

        // Capacités de base
        canDoubleJump = false;
        canDash = false;
        canGlide = false;

        // Massue désactivée par défaut
        if (massue != null)
            massue.SetActive(false);
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (IsDashing)
            return;
        
        // Timer chute
        if (!isGrounded)
        {
            fallTimer += Time.fixedDeltaTime;
        }
        else
        {
            if (enableFallingImpact && fallTimer > fallImpactTime)
            {
                TriggerFallImpact();
            }

            fallTimer = 0f;
        }

        IsRunning = canRun && Input.GetKey(runningKey);
        float targetSpeed = IsRunning ? runSpeed : speed;

        if (speedOverrides.Count > 0)
            targetSpeed = speedOverrides[^1]();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v);
        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        Vector3 moveDir = transform.TransformDirection(inputDir);
        Vector3 desiredVelocity = moveDir * targetSpeed;

        Vector3 currentHorizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        Vector3 velocityDiff = desiredVelocity - currentHorizontalVelocity;

        // Accélération différente sol / air
        float accel = IsGrounded() ? groundAcceleration : airAcceleration;

        // Limite la conservation de vitesse en l’air
        if (!IsGrounded())
        {
            velocityDiff = Vector3.ClampMagnitude(velocityDiff, maxAirSpeed);
            velocityDiff *= currentAirControl;
        }
        
        rb.AddForce(velocityDiff * accel * Time.fixedDeltaTime, ForceMode.VelocityChange);

        ApplyBetterGravity();
    }
    void ApplyBetterGravity()
    {
        float baseGravity = 9.81f * gravityMultiplier;

        if (rb.linearVelocity.y < 0)
        {
            // Chute rapide + Oni lourd
            rb.AddForce(
                Vector3.down * baseGravity * (fallGravityMultiplier - 1f),
                ForceMode.Acceleration
            );
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space) || !canJump)
        {
            // Petit saut + Oni lourd
            rb.AddForce(
                Vector3.down * baseGravity * (lowJumpGravityMultiplier - 1f),
                ForceMode.Acceleration
            );
        }
    }




    void Update()
    {
        // Vérification sol
        isGrounded = groundCheck.isGrounded;

        // Attaque Oni
        if (canAttack && Input.GetMouseButtonDown(0))
            Attack();
    }

    // --- Impact de chute ---
    private void TriggerFallImpact()
    {
        Debug.Log("Feur");

        Collider[] hits = Physics.OverlapSphere(transform.position, fallImpactRadius);

        foreach (Collider hit in hits)
        {
            // Dégâts aux ennemis
            Health h = hit.GetComponent<Health>();
            if (h != null)
            {
                float damage = fallImpactDamage * damageMultiplier;
                h.TakeDamage(damage);
                Debug.Log($"[ONI] {hit.name} prend {damage} dégâts de l’impact");
            }

            // Destruction murs / boucliers
            if (hit.CompareTag("Breakable") && canBreakShield)
                Destroy(hit.gameObject);
        }
    }

    // --- Attaque Oni ---
    public void Attack()
    {
        if (!canAttack) return;

        if (animator != null && attackClip1 != null && attackClip2 != null)
        {
            AnimationClip clip = Random.value > 0.5f ? attackClip1 : attackClip2;
            animator.Play(clip.name);
        }
    }

    // --- Animation Event pour dégâts ---
    public void ApplyAttackDamage()
    {
        if (!canAttack) return;

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * attackRange, attackRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            Health h = hit.GetComponent<Health>();
            if (h != null)
            {
                float damage = attackDamage * damageMultiplier;
                h.TakeDamage(damage);
                Debug.Log($"[ONI] {hit.name} prend {damage} dégâts via Animation Event");
            }

            if (hit.CompareTag("Breakable") && canBreakShield)
                Destroy(hit.gameObject);
        }
    }

    bool IsGrounded()
    {
        // Simple check, à remplacer par ton GroundCheck si besoin
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            1.1f
        );
    }

    public void Stun(float time)
    {
        StartCoroutine(StunCoroutine(time));
    }

    IEnumerator StunCoroutine(float time)
    {
        canRun = false;
        yield return new WaitForSeconds(time);
        canRun = true;
    }

    public Rigidbody Rigidbody => rb;
    
    [HideInInspector] public Coroutine airControlCoroutine;

    public void StartAirControlRecovery(float recoveryTime)
    {
        if (airControlCoroutine != null)
        {
            StopCoroutine(airControlCoroutine);
            airControlCoroutine = null;
        }

        currentAirControl = 0f;
        airControlCoroutine = StartCoroutine(RestoreAirControlSmooth(recoveryTime));
    }

    IEnumerator RestoreAirControlSmooth(float recoveryTime)
    {
        float t = 0f;
        float start = 0f;
        float end = airControlMultiplier;

        while (t < recoveryTime)
        {
            t += Time.deltaTime;
            currentAirControl = Mathf.Lerp(start, end, t / recoveryTime);
            yield return null;
        }

        currentAirControl = end;
        airControlCoroutine = null;
    }

    // Visualisation dans l’éditeur
    void OnDrawGizmosSelected()
    {
        if (enableFallingImpact)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fallImpactRadius);
        }

        if (canAttack)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRange);
        }
    }
}
