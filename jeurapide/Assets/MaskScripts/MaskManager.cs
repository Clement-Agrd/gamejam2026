using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [Header("Masques")]
    public Mask[] masks;

    public MaskUIWheel uiWheel;
    public int CurrentIndex => currentIndex;

    [Header("UI Transition")]
    public MaskTransitionUI transitionUI;

    [Header("Audio")]
    public AudioSource maskChangeAudio;

    private Mask currentMask;
    private FirstPersonMovement player;
    private int currentIndex = 0;

    void Start()
    {
        player = GetComponent<FirstPersonMovement>();

        if (player == null)
        {
            Debug.LogError("[MaskManager] PlayerController introuvable");
            return;
        }

        foreach (Mask mask in masks)
        {
            if (mask != null)
                mask.Init(player);
        }

        if (uiWheel != null)
            uiWheel.Init(this);

        if (masks.Length > 0)
            ChangeMask(0, Color.white);
    }

    void Update()
    {
        HandleScrollInput();
        HandleKeyboardInput();
    }

    void HandleScrollInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            NextMask();
        else if (scroll < 0f)
            PreviousMask();
    }

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

        // 🔊 SON DE CHANGEMENT DE MASQUE (UN SEUL SON)
        if (maskChangeAudio != null)
            maskChangeAudio.Play();

        if (uiWheel != null)
            uiWheel.Refresh();
    }
}
