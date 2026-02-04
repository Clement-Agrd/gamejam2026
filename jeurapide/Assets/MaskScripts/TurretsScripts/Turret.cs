using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Aim")]
    public float aimHeight = 1.5f; // hauteur à viser (tête / torse)

    [Header("Target")]
    public Transform player;

    [Header("Rotation")]
    public Transform rotatingPart;
    public float rotationSpeed = 5f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    [Header("Audio")]
    public AudioSource shootAudio;

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

            // viser un point au-dessus des pieds du joueur
            Vector3 targetPos = player.position + Vector3.up * aimHeight;

            Vector3 dir = (targetPos - firePoint.position).normalized;
            firePoint.rotation = Quaternion.LookRotation(dir);

            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // 🔊 SON DE TIR (UN SEUL SON)
            if (shootAudio != null)
                shootAudio.Play();
        }
    }



    public void SetActive(bool value, Transform target)
    {
        active = value;
        player = value ? target : null;
    }
}