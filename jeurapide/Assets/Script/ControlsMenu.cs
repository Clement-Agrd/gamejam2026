using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public GameObject panelControles;

    void Start()
    {
        panelControles.SetActive(false);
    }
    public void Ouvrir()
    {
        panelControles.SetActive(true);
    }

    public void Fermer()
    {
        panelControles.SetActive(false);
    }
    
}