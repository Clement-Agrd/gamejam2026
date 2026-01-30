using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskTransitionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image screenBorder; // Image couvrant l'écran pour le contour
    public float fadeDuration = 0.5f; // durée du fondu

    private Coroutine currentCoroutine;

    public void PlayTransition(Color maskColor)
    {
        if (screenBorder == null)
        {
            Debug.LogWarning("[MaskTransitionUI] screenBorder non assigné !");
            return;
        }

        Color startColor = maskColor;
        startColor.a = 0f; // commence transparent

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(FadeBorder(startColor, maskColor));
    }

    private IEnumerator FadeBorder(Color startColor, Color targetColor)
    {
        float timer = 0f;
        screenBorder.color = startColor;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Alpha progressif pour fondu
            Color c = Color.Lerp(startColor, targetColor, t);
            screenBorder.color = c;

            yield return null;
        }

        // Retour à alpha = 0
        Color endColor = targetColor;
        endColor.a = 0f;
        screenBorder.color = endColor;

        currentCoroutine = null;
    }
}