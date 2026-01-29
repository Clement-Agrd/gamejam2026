public class KitsuneMask : Mask
{
    public override void Activate()
    {
        player.canFight = false;
        player.canSeeInvisible = true;
        player.canPassThruWall = true;
        player.canWallRun = true;
    }

    public override void Deactivate()
    {
        player.canFight = true;
        player.canSeeInvisible = false;
        player.canPassThruWall = false;
        player.canWallRun = false;
    }
}