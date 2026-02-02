using UnityEngine;

public class OniMask : Mask
{
    [Header("Oni Settings")]
    [Range(0.1f, 2f)]
    public float speedMultiplier = 0.6f;
    [Range(1f, 5f)]
    public float oniGravityMultiplier = 2f;

    public float damageMultiplierValue = 2f;    
    public float damageReductionValue = 0.5f;

    public float fallImpactTimeValue = 0.1f;
    public float fallImpactRadiusValue = 3f;
    public float fallImpactDamageValue = 50f;

    public override void Activate()
    {
        player.canJump = false;
        player.speed *= speedMultiplier;
        player.runSpeed *= speedMultiplier;
        player.gravityMultiplier = oniGravityMultiplier;

        player.damageMultiplier = damageMultiplierValue;
        player.canBreakShield = true;

        player.enableFallingImpact = true;
        player.fallImpactTime = fallImpactTimeValue;
        player.fallImpactRadius = fallImpactRadiusValue;
        player.fallImpactDamage = fallImpactDamageValue;

        if (player.massue != null)
            player.massue.SetActive(true);
    }

    public override void Deactivate()
    {
        player.canJump = true;
        player.speed /= speedMultiplier;
        player.runSpeed /= speedMultiplier;
        player.gravityMultiplier = 1f;

        player.damageMultiplier = 1f;
        player.canBreakShield = false;

        player.enableFallingImpact = false;

        if (player.massue != null)
            player.massue.SetActive(false);
    }
}