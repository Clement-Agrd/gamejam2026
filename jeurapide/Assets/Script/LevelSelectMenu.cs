using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelLevelSelect;

    void Start()
    {

        panelLevelSelect.SetActive(false);
    }
    public void OuvrirSelectionNiveau()
    {
        panelLevelSelect.SetActive(true);
    }

    public void FermerSelectionNiveau()
    {
        panelLevelSelect.SetActive(false);
    }

    public void ChargerNiveau(string nomScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomScene);
    }
}