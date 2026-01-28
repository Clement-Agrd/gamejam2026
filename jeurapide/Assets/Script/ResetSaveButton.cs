using UnityEngine;

public class ResetSaveButton : MonoBehaviour
{
    public void ResetSave()
    {
        // Supprime TOUTES les PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();


        Debug.Log("Sauvegarde réinitialisée");
    }
}