using UnityEngine;

public class finirleniveauen1click : MonoBehaviour
{
    public LevelEnd levelEnd;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            levelEnd.FinDuNiveau();
        }
    }
}
