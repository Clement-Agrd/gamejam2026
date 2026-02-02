using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    private PlayerRespawn respawn;

    void Awake()
    {
        currentHealth = maxHealth;
        respawn = GetComponent<PlayerRespawn>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} a pris {amount} dégâts. HP restant : {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");

        // Si c'est le player → respawn
        if (respawn != null)
        {
            respawn.Respawn();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 👇 NOUVELLE FONCTION
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}