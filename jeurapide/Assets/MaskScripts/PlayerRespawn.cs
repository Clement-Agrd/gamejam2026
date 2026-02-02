using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    public void Respawn()
    {
        // Téléportation
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        // Reset de la vie
        health.ResetHealth();

        Debug.Log("Player respawn");
    }
}