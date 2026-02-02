using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [Header("Masques")]
    public Mask[] masks;
    
    public MaskUIWheel uiWheel;
    public int CurrentIndex => currentIndex;


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
       
        if (uiWheel != null)
            uiWheel.Init(this);


        // Activer le premier masque par défaut
        if (masks.Length > 0)
            ChangeMask(0, Color.white);
    }

    void Update()
    {
        HandleScrollInput();
        HandleKeyboardInput(); // 👈 NOUVEAU
    }

    // ================= MOLETTE =================
    void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            NextMask();
        }
        else if (scroll < 0f)
        {
            PreviousMask();
        }
    }

    // ================= CLAVIER =================
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectMask(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectMask(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectMask(2);
    }

    void SelectMask(int index)
    {
        if (index < 0 || index >= masks.Length)
            return;

        currentIndex = index;
        ChangeMask(currentIndex, Color.white);
    }

    // ================= LOGIQUE MASQUES =================
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
            return;

        if (currentMask != null)
            currentMask.Deactivate();

        currentMask = masks[index];

        if (transitionUI != null)
            transitionUI.PlayTransition(transitionColor);

        currentMask.Activate();

        Debug.Log($"[MaskManager] Masque activé : {currentMask.name}");
       
        if (uiWheel != null)
            uiWheel.Refresh();

    }
}
