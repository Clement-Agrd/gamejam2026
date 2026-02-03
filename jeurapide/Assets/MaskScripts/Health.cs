using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (CompareTag("Player"))
        {
            // Recharge la scène actuelle
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Si ce n'est pas le joueur → on détruit juste l'objet
            Destroy(gameObject);
        }
    }

    // 👇 NOUVELLE FONCTION
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}