using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public float damage = 10f;
    public float damageCooldown = 0.5f; // temps entre deux dégâts

    private float lastDamageTime = -999f;

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryDamage(collision.collider);
    }

    private void TryDamage(Collider other)
    {
        // Vérifie le tag Player
        if (!other.CompareTag("Player"))
            return;

        // Cooldown
        if (Time.time < lastDamageTime + damageCooldown)
            return;

        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}