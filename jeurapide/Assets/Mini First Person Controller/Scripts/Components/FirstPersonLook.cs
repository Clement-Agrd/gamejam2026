using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform character;

    [Header("Look Settings")]
    public float sensitivity = 2f;

    private Vector2 rotation;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sensitivity = PlayerPrefs.GetFloat("SensibiliteSouris", sensitivity);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

        }

        // Bloque le look si le menu est ouvert
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
        // Sensibilité en temps réel
        float currentSensitivity = PlayerPrefs.GetFloat("SensibiliteSouris", sensitivity);

        float mouseX = Input.GetAxisRaw("Mouse X") * currentSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * currentSensitivity;

        rotation.x += mouseX;
        rotation.y += mouseY;
        rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);

        transform.localRotation = Quaternion.Euler(-rotation.y, 0f, 0f);
        character.localRotation = Quaternion.Euler(0f, rotation.x, 0f);
    }
}