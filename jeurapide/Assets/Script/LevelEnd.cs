using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public int niveauActuel = 1;
    public GameObject PanelFinDuNiveau;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PanelFinDuNiveau.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void FermerFinDuNiveau()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PanelFinDuNiveau.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void FinDuNiveau()
    {
        Cursor.lockState = CursorLockMode.None;
        LevelProgression.DebloquerNiveau(niveauActuel + 1);
        PanelFinDuNiveau.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }
}