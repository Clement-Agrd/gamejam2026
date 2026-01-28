using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public int numeroNiveau;
    public string nomScene;
    public Button button;
    public GameObject lockIcon; // optionnel

    void Start()
    {
        bool debloque = LevelProgression.EstDebloque(numeroNiveau);

        button.interactable = debloque;

        if (lockIcon != null)
            lockIcon.SetActive(!debloque);
    }

    public void ChargerNiveau()
    {
        if (!LevelProgression.EstDebloque(numeroNiveau))
            return;
        
        SceneManager.LoadScene(nomScene);
    }
}