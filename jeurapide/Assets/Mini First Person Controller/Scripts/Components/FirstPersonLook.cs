using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform character;

    [Header("Look Settings")]
    public float sensitivity = 2f;
    public float smoothing = 1.5f;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    void Reset()
    {
        // Récupère automatiquement le personnage parent
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        // Verrouille la souris
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Charge la sensibilité sauvegardée
        sensitivity = PlayerPrefs.GetFloat("SensibiliteSouris", sensitivity);
    }

    void Update()
    {
        // Bloque le look si le menu est ouvert
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        // Lecture de la sensibilité en temps réel
        float currentSensitivity = PlayerPrefs.GetFloat("SensibiliteSouris", sensitivity);

        // Entrées souris (ancien Input)
        Vector2 mouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y")
        );

        // Calcul du mouvement avec smoothing
        Vector2 rawFrameVelocity = mouseDelta * currentSensitivity;
        frameVelocity = Vector2.Lerp(
            frameVelocity,
            rawFrameVelocity,
            Time.deltaTime * (1f / smoothing)
        );

        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90f, 90f);

        // Application des rotations
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}