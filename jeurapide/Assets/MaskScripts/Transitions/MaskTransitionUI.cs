using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskTransitionUI : MonoBehaviour
{
    public Image overlay;
    public float fadeDuration = 0.25f;

    public void PlayTransition(Color color)
    {
        Debug.Log("[UI] Transition OK");
        StopAllCoroutines();
        StartCoroutine(Fade(color));
    }

    IEnumerator Fade(Color color)
    {
        // Fade in
        color.a = 1f;
        overlay.color = color;
        yield return new WaitForSeconds(0.05f);

        // Fade out
        color.a = 0f;
        overlay.color = color;
    }
}