using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelOptions;

    [Header("UI")]
    public Slider sensibiliteSlider;



    void Start()
    {
        // Sensibilité
        sensibiliteSlider.value = PlayerPrefs.GetFloat("SensibiliteSouris", 2f);

        panelOptions.SetActive(false);
    }
    

    public void AppliquerSensibilite(float value)
    {
        PlayerPrefs.SetFloat("SensibiliteSouris", value);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (panelOptions.activeSelf)
                FermerMenu();
            else
                OuvrirMenu();
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
    public void FermerMenuN()
    {
        panelOptions.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
