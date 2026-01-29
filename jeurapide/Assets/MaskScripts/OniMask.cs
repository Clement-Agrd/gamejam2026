using UnityEngine;

public class OniMask : Mask
{
    [Header("Oni Settings")]
    [Range(0.1f, 2f)]
    public float speedMultiplier = 0.6f;        // Multiplie la vitesse du player
    [Range(1f, 5f)]
    public float oniGravityMultiplier = 2f;     // Multiplie la gravité pour lourdeur

    public float damageMultiplierValue = 2f;    
    public float damageReductionValue = 0.5f;

    public float fallImpactTimeValue = 1f;      // Durée chute pour impact
    public float fallImpactRadiusValue = 3f;    // Rayon impact
    public float fallImpactDamageValue = 50f;   // Dégâts impact

    public override void Activate()
    {
        // Capacités et stats
        player.canJump = false;                            // Pas de saut
        player.speed *= speedMultiplier;                  // Multiplie la vitesse
        player.gravityMultiplier = oniGravityMultiplier; // Gravité lourde

        player.damageMultiplier = damageMultiplierValue;
        player.damageReduction = damageReductionValue;
        player.canBreakShield = true;

        // Dégâts de chute Oni
        player.enableFallingImpact = true;
        player.fallImpactTime = fallImpactTimeValue;
        player.fallImpactRadius = fallImpactRadiusValue;
        player.fallImpactDamage = fallImpactDamageValue;

        // Massue visible uniquement pour Oni
        if (player.massue != null)
            player.massue.SetActive(true);

        Debug.Log("[ONI MASK] Activé - vitesse & gravité configurables");
    }

    public override void Deactivate()
    {
        player.canJump = true;
        player.speed /= speedMultiplier;                  // Retour à la vitesse normale
        player.gravityMultiplier = 1f;                   // Gravité normale

        player.damageMultiplier = 1f;
        player.damageReduction = 0f;
        player.canBreakShield = false;

        player.enableFallingImpact = false;

        // Massue invisible
        if (player.massue != null)
            player.massue.SetActive(false);

        Debug.Log("[ONI MASK] Désactivé - retour normal");
    }
}
