using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float runSpeed = 9f;
    public float airControlMultiplier = 0.5f;

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
    public bool canPhase = false;
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

    private float fallTimer = 0f;
    private Rigidbody rb;

    public bool IsDashing { get; set; }
    /// <summary>
    /// Functions to override movement speed (last added wins)
    /// </summary>
    public List<System.Func<float>> speedOverrides = new();

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Paramètres physiques conseillés
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

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
        // Bloque le mouvement pendant le dash
        if (IsDashing)
            return;

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

        // Vitesse horizontale actuelle
        Vector3 currentHorizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );
        // Mouvement horizontal
        Vector3 horizontalVelocity = transform.rotation * new Vector3(x * targetSpeed, 0f, z * targetSpeed);

        // Gravité personnalisée (Oni lourd)
        if (gravityMultiplier > 1f)
            rb.AddForce(Vector3.down * 9.81f * (gravityMultiplier - 1f), ForceMode.Acceleration);

        // Applique horizontal + vertical (y conservée pour gravité)
        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);

        // Timer chute
        if (!isGrounded)
            fallTimer += Time.fixedDeltaTime;
        else
        {
            if (enableFallingImpact && fallTimer >= fallImpactTime)
                TriggerFallImpact();
            fallTimer = 0f;
        }
        // Différence à corriger
        Vector3 velocityChange = desiredVelocity - currentHorizontalVelocity;

        // Contrôle réduit en l'air
        if (!IsGrounded())
            velocityChange *= airControlMultiplier;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Update()
    {
        // Vérification sol
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Attaque Oni
        if (canAttack && enableFallingImpact && Input.GetMouseButtonDown(0))
            Attack();
    }

    // --- Impact de chute ---
    private void TriggerFallImpact()
    {
        Debug.Log("[ONI] Impact de chute !");

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
