public class KitsuneMask : Mask
{
    public override void Activate()
    {
        player.canFight = false;
        player.canSeeInvisible = true;
        player.canPhase = true;
    }

    public override void Deactivate()
    {
        player.canFight = true;
        player.canSeeInvisible = false;
        player.canPhase = false;
    }
}