using UnityEngine;
using System.Collections;

public class TenguMask : Mask
{
   

    public override void Activate()
    {
        player.canDoubleJump = true;
        player.canDash = true;
        player.canGlide = true;
    }

    public override void Deactivate()
    {
        player.canDoubleJump = false;
        player.canDash = false;
        player.canGlide = false;
    }
  
}