using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OptionsMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelOptions;

    [Header("UI")]
    public Slider luminositeSlider;
    public Slider sensibiliteSlider;

    [Header("Post Processing")]
    public Volume volume;
    private LiftGammaGain liftGammaGain;

    void Start()
    {
        // Gamma
        if (volume.profile.TryGet(out liftGammaGain))
        {
            float savedGamma = PlayerPrefs.GetFloat("Luminosite", 1f);
            luminositeSlider.value = savedGamma;
            liftGammaGain.gamma.value = new Vector4(savedGamma, savedGamma, savedGamma, 0f);
        }

        // Sensibilité
        sensibiliteSlider.value = PlayerPrefs.GetFloat("SensibiliteSouris", 2f);

        // Menu fermé au départ
        panelOptions.SetActive(false);
    }

    public void AppliquerLuminosite(float value)
    {
        if (liftGammaGain != null)
        {
            liftGammaGain.gamma.value = new Vector4(0, 0, 0, value);
            PlayerPrefs.SetFloat("Luminosite", value);
        }
    }

    public void AppliquerSensibilite(float value)
    {
        PlayerPrefs.SetFloat("SensibiliteSouris", value);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (panelOptions.activeSelf)
            {
                FermerMenu();
            }
            else
            {
                OuvrirMenu();
            }
        }
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
    }
    
}