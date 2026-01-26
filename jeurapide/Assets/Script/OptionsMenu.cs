using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI")]
    public Slider luminositeSlider;
    public Slider sensibiliteSlider;
    public Image overlayLuminosite;

    [Header("Panels")]
    public GameObject panelOptions;

    [Header("Souris")]
    public static float sensibiliteSouris = 2f;

    void Start()
    {
        panelOptions.SetActive(false);
        // Charger les valeurs sauvegardées
        luminositeSlider.value = PlayerPrefs.GetFloat("Luminosite", 0.2f);
        sensibiliteSlider.value = PlayerPrefs.GetFloat("SensibiliteSouris", 2f);

        AppliquerLuminosite(luminositeSlider.value);
        AppliquerSensibilite(sensibiliteSlider.value);
    }

    public void AppliquerLuminosite(float value)
    {
        Color c = overlayLuminosite.color;
        c.a = value;
        overlayLuminosite.color = c;

        PlayerPrefs.SetFloat("Luminosite", value);
    }

    public void AppliquerSensibilite(float value)
    {
        sensibiliteSouris = value;
        PlayerPrefs.SetFloat("SensibiliteSouris", value);
    }

    public void OuvrirMenu()
    {
        panelOptions.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FermerMenu()
    {
        panelOptions.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}