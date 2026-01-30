using Unity.VisualScripting;
using UnityEngine;

public class LaSourisDisparaitPasStp : MonoBehaviour
{


    public void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}