using System.Collections;
using UnityEngine;

[RequireComponent(typeof(FirstPersonMovement))]
public class DashAbility : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 8f;
    public float dashSpeed = 50f;       // vitesse pour interpolation
    public float dashCooldown = 1f;
    public float damage = 25f;
    public KeyCode dashKey = KeyCode.E;

    [Header("References")]
    public Transform cameraTransform;

    [Header("Physics")]
    public bool disableGravityDuringDash = true;
    public LayerMask enemyLayer;      // ennemis à toucher
    public LayerMask obstacleLayer;   // murs / obstacles qui bloquent

    private FirstPersonMovement player;
    private bool canDashNow = true;

    void Awake()
    {
        player = GetComponent<FirstPersonMovement>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!player.canDash || !canDashNow)
            return;

        if (Input.GetKeyDown(dashKey))
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDashNow = false;
        player.IsDashing = true;

        Rigidbody rb = player.Rigidbody;

        if (disableGravityDuringDash)
            rb.useGravity = false;

        Vector3 dashDir = cameraTransform.forward.normalized;
        float remainingDistance = dashDistance;

        while (remainingDistance > 0f)
        {
            float step = dashSpeed * Time.deltaTime;

            // Vérifier collision avec obstacle
            if (rb.SweepTest(dashDir, out RaycastHit hitObstacle, step, QueryTriggerInteraction.Ignore))
            {
                remainingDistance = 0f; // stop dash
                break;
            }

            // Vérifier ennemis sur le chemin
            if (Physics.Raycast(rb.position, dashDir, out RaycastHit hitEnemy, step, enemyLayer))
            {
                Health health = hitEnemy.collider.GetComponent<Health>();
                if (health != null)
                    health.TakeDamage(damage);
            }

            rb.MovePosition(rb.position + dashDir * step);
            remainingDistance -= step;
            yield return null;
        }

        if (disableGravityDuringDash)
            rb.useGravity = true;

        player.IsDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDashNow = true;
    }
}