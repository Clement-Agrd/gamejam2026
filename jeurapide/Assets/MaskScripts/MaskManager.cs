using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [Header("Masques")]
    public Mask[] masks;

    [Header("UI Transition")]
    public MaskTransitionUI transitionUI;

    private Mask currentMask;
    private FirstPersonMovement player;
    private int currentIndex = 0;

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

        // Activer le premier masque par défaut
        if (masks.Length > 0)
            ChangeMask(0, Color.white);
    }

    void Update()
    {
        // Détection de la molette
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            // Molette vers le haut -> masque suivant
            NextMask();
        }
        else if (scroll < 0f)
        {
            // Molette vers le bas -> masque précédent
            PreviousMask();
        }
    }

    void NextMask()
    {
        if (masks.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % masks.Length;
        ChangeMask(currentIndex, Color.white);
    }

    void PreviousMask()
    {
        if (masks.Length == 0)
            return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = masks.Length - 1;

        ChangeMask(currentIndex, Color.white);
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

        Debug.Log($"[MaskManager] Masque activé : {currentMask.name}");
    }
}
