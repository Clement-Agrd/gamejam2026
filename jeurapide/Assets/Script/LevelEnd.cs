using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public int niveauActuel = 1;

    public void FinDuNiveau()
    {
        LevelProgression.DebloquerNiveau(niveauActuel + 1);
    }
}