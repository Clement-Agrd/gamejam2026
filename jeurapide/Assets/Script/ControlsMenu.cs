using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public GameObject panelControles;

    public void Ouvrir()
    {
        panelControles.SetActive(true);
    }

    public void Fermer()
    {
        panelControles.SetActive(false);
    }
    
}