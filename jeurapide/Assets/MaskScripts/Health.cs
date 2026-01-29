using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Fonction pour infliger des dégâts
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} a pris {amount} dégâts. HP restant : {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Fonction appelée quand l'objet meurt
    private void Die()
    {
        Debug.Log($"{gameObject.name} est mort !");
        Destroy(gameObject);
    }
}