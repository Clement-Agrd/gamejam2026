using UnityEngine;

public abstract class Mask : MonoBehaviour
{
    protected FirstPersonMovement player;

    public virtual void Init(FirstPersonMovement pc) 
    {
        player = pc;
    }

    public abstract void Activate();
    public abstract void Deactivate();
    
}