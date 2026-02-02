using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Rotation")]
    public Transform rotatingPart;
    public float rotationSpeed = 5f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private bool active = false;
    private float fireTimer;

    void Update()
    {
        if (!active || player == null)
            return;

        RotateTurret();
        HandleShooting();
    }

    void RotateTurret()
    {
        Vector3 direction = player.position - rotatingPart.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rotatingPart.rotation = Quaternion.Slerp(
            rotatingPart.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void SetActive(bool value, Transform target)
    {
        active = value;
        player = value ? target : null;
    }
}