using UnityEngine;

public class OniWeaponHitbox : MonoBehaviour
{
    [HideInInspector] public FirstPersonMovement player;
    public float radius = 1f;                 
    public LayerMask enemyLayer;              
    private bool canDealDamage = false;

    public void DealDamage()
    {
        if (!canDealDamage || player == null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider hit in hits)
        {
            Health h = hit.GetComponent<Health>();
            if (h != null)
            {
                float damage = player.attackDamage * player.damageMultiplier;
                h.TakeDamage(damage);
                Debug.Log($"[ONI] {hit.name} prend {damage} dégâts via attaque physique");
            }

            if (hit.CompareTag("Breakable") && player.canBreakShield)
                Destroy(hit.gameObject);
        }
    }

    public void EnableDamage() => canDealDamage = true;
    public void DisableDamage() => canDealDamage = false;
}