public class OniMask : Mask
{
    public override void Activate()
    {
        player.canJump = false;
        player.speed *= 0.6f;
        player.damageMultiplier = 2f;
        player.damageReduction = 0.5f;
    }

    public override void Deactivate()
    {
        player.canJump = true;
        player.speed /= 0.6f;
        player.damageMultiplier = 1f;
        player.damageReduction = 0f;
    }
}