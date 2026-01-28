using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [Header("Masques")]
    public Mask[] masks;

    [Header("UI Transition")]
    public MaskTransitionUI transitionUI;

    private Mask currentMask;
    private FirstPersonMovement player;

    void Start()
    {
        // RÉFÉRENCE PLAYER
        player = GetComponent<FirstPersonMovement>();

        if (player == null)
        {
            Debug.LogError("[MaskManager] PlayerController introuvable");
            return;
        }

        // INIT DES MASQUES
        foreach (Mask mask in masks)
        {
            if (mask != null)
                mask.Init(player);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("[INPUT] 1 → TENGU");
            ChangeMask(0, Color.softBlue);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("[INPUT] 2 → KITSUNE");
            ChangeMask(1, new Color(1f, 0.8f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("[INPUT] 3 → ONI");
            ChangeMask(2, Color.red);
        }
    }

    void ChangeMask(int index, Color transitionColor)
    {
        if (index < 0 || index >= masks.Length)
            return;

        if (currentMask == masks[index])
            return; // même masque -> ne rien changer

        if (currentMask != null)
            currentMask.Deactivate();

        currentMask = masks[index];

        if (transitionUI != null)
            transitionUI.PlayTransition(transitionColor);

        currentMask.Activate();
    }
}