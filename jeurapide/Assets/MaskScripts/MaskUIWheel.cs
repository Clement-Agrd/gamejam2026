using UnityEngine;
using UnityEngine.UI;

public class MaskUIWheel : MonoBehaviour
{
    public Image topImage;
    public Image currentImage;
    public Image bottomImage;

    private MaskManager manager;

    public void Init(MaskManager m)
    {
        manager = m;
        Refresh();
    }

    public void Refresh()
    {
        if (manager == null || manager.masks.Length == 0)
            return;

        int current = manager.CurrentIndex;
        Mask[] masks = manager.masks;

        int top = (current + 1) % masks.Length;
        int bottom = current - 1;
        if (bottom < 0) bottom = masks.Length - 1;

        currentImage.sprite = masks[current].icon;
        topImage.sprite = masks[top].icon;
        bottomImage.sprite = masks[bottom].icon;

        currentImage.enabled = true;
        topImage.enabled = true;
        bottomImage.enabled = true;
    }
}