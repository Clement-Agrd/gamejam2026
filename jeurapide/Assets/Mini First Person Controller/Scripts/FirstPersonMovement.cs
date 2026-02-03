using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private GroundCheck groundCheck;

    [Header("Movement")]
    public float speed = 7f;
    public float runSpeed = 12f;
    public KeyCode runningKey = KeyCode.LeftShift;
    public float groundAcceleration = 40f;
    public float airAcceleration = 15f;
    public float maxAirSpeed = 12f;
    
    [Header("Jump Hold")]
    public float maxJumpHoldTime = 0.8f;
    private float jumpHoldTimer = 0f;

    

    [Header("Gravity")]
    public float gravityMultiplier = 1f;
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;

    [Header("Air Control")]
    public float airControlMultiplier = 0.5f;
    public float airControlRecoveryTime = 1f;
    [HideInInspector] public float currentAirControl;

    [Header("Abilities")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public bool canJump = true;
    public bool canDoubleJump = false;
    public bool canDash = false;
    public bool canGlide = false;
    public bool canFight = false;
    public bool canPassThruWall = false;
    public bool canWallRun = false;
    public bool IsWallRunning { get; set; }
    public bool canBreakShield = false;

    [Header("Combat Stats")]
    public float damageMultiplier = 1f;
    public float attackDamage = 20f;

    [Header("Falling Impact")]
    public bool enableFallingImpact = false;
    public float fallImpactTime = 1f;
    public float fallImpactRadius = 3f;
    public float fallImpactDamage = 50f;
    public LayerMask fallImpactLayers; // <-- AJOUT


    [Header("Oni Weapon")]
    public GameObject massue;               // Ton objet massue
    public Transform weaponPivot;           // Empty GameObject pivot pour faire tourner la massue
    public float swingAngle = 120f;         // Largeur du swing (droite à gauche)
    public float swingDuration = 0.3f;
    public float attackCooldown = 0.6f;
    public float hitDistance = 1.5f;
    public float hitRadius = 1.2f;
    public LayerMask enemyLayer;

    [HideInInspector] public bool isGrounded = true;

    private Rigidbody rb;
    private float fallTimer = 0f;
    public bool IsDashing { get; set; }

    private bool isAttacking = false;
    private float lastAttackTime = -999f;

    [HideInInspector] public Coroutine airControlCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (groundCheck == null)
            groundCheck = GetComponentInChildren<GroundCheck>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        currentAirControl = airControlMultiplier;

        canDoubleJump = false;
        canDash = false;
        canGlide = false;

        if (massue != null)
            massue.SetActive(false);   // La massue est visible et pivotée
    }

    void Update()
    {
        // Vérification sol
        isGrounded = groundCheck.isGrounded;

        IsRunning = canRun && Input.GetKey(runningKey);

        if (canFight && Input.GetMouseButtonDown(0))
            TryAttack();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ApplyBetterGravity();
    }


    private void HandleMovement()
    {
        if (IsDashing) return;
        
        if (isGrounded)
            jumpHoldTimer = 0f;

        if (!isGrounded) fallTimer += Time.fixedDeltaTime;
        else
        {
            if (enableFallingImpact && fallTimer > fallImpactTime)
                TriggerFallImpact();
            fallTimer = 0f;
        }

        float targetSpeed = IsRunning ? runSpeed : speed;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0f, v);
        if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        Vector3 moveDir = transform.TransformDirection(inputDir);
        Vector3 desiredVelocity = moveDir * targetSpeed;

        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 velocityDiff = desiredVelocity - currentHorizontalVelocity;

        float accel = isGrounded ? groundAcceleration : airAcceleration;

        if (!isGrounded)
        {
            velocityDiff = Vector3.ClampMagnitude(velocityDiff, maxAirSpeed);
            velocityDiff *= currentAirControl;
        }

        rb.AddForce(velocityDiff * accel * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    private void ApplyBetterGravity()
    {
        float baseGravity = 9.81f * gravityMultiplier;

        // Si on monte
        if (rb.linearVelocity.y > 0)
        {
            // Si on tient espace ET qu'on peut jump → on compte le temps
            if (Input.GetKey(KeyCode.Space) && canJump)
            {
                jumpHoldTimer += Time.fixedDeltaTime;

                // Si on dépasse la durée max → on force la low gravity
                if (jumpHoldTimer > maxJumpHoldTime)
                {
                    rb.AddForce(Vector3.down * baseGravity * (lowJumpGravityMultiplier - 1f), ForceMode.Acceleration);
                }
            }
            else
            {
                // Pas espace → low jump direct
                rb.AddForce(Vector3.down * baseGravity * (lowJumpGravityMultiplier - 1f), ForceMode.Acceleration);
            }
        }

        // Si on descend
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.down * baseGravity * (fallGravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    

    // --- Impact de chute ---
    private void TriggerFallImpact()
    {
        Debug.Log("Boumm");
        Collider[] hits = Physics.OverlapSphere
        (
            transform.position,
            fallImpactRadius,
            fallImpactLayers,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody == rb) 
                continue; // ignore le player

            Health h = hit.GetComponent<Health>();
            if (h != null)
            {
                float damage = fallImpactDamage * damageMultiplier;
                h.TakeDamage(damage);
            }

            if (hit.CompareTag("Breakable") && canBreakShield)
                Destroy(hit.gameObject);
        }

    }

    
    private void TryAttack()
    {
        if (isAttacking) return;
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (weaponPivot == null) return;

        StartCoroutine(SwingWeapon());
    }

    private IEnumerator SwingWeapon()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        Quaternion startRot = Quaternion.Euler(0f, swingAngle / 2f, 0f);   // droite
        Quaternion endRot = Quaternion.Euler(0f, -swingAngle / 2f, 0f);    // gauche
        weaponPivot.localRotation = startRot;

        HashSet<Health> hitEnemies = new HashSet<Health>();

        float t = 0f;
        while (t < swingDuration)
        {
            t += Time.deltaTime;
            float fraction = t / swingDuration;
            weaponPivot.localRotation = Quaternion.Slerp(startRot, endRot, fraction);

            // Hit check
            Vector3 hitCenter = weaponPivot.position + weaponPivot.forward * hitDistance;
            Collider[] hits = Physics.OverlapSphere(hitCenter, hitRadius, enemyLayer);

            foreach (Collider hit in hits)
            {
                Health h = hit.GetComponent<Health>();
                if (h != null && !hitEnemies.Contains(h))
                {
                    h.TakeDamage(attackDamage * damageMultiplier);
                    hitEnemies.Add(h);
                    Debug.Log($"[ONI] {hit.name} touché pour {attackDamage * damageMultiplier}");
                }
            }

            yield return null;
        }

        weaponPivot.localRotation = Quaternion.identity;
        isAttacking = false;
    }



    public void StartAirControlRecovery(float recoveryTime)
    {
        if (airControlCoroutine != null)
            StopCoroutine(airControlCoroutine);

        currentAirControl = 0f;
        airControlCoroutine = StartCoroutine(RestoreAirControlSmooth(recoveryTime));
    }

    private IEnumerator RestoreAirControlSmooth(float recoveryTime)
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


    public Rigidbody Rigidbody => rb;

    void OnDrawGizmosSelected()
    {
        if (weaponPivot != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 hitCenter = weaponPivot.position + weaponPivot.forward * hitDistance;
            Gizmos.DrawWireSphere(hitCenter, hitRadius);
        }

        if (enableFallingImpact)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fallImpactRadius);
        }
    }
}
