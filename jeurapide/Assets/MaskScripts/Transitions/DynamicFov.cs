using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DynamicFOV : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Rigidbody du player")]
    public Rigidbody playerRigidbody;

    private Camera cam;

    [Header("FOV Settings")]
    public float baseFOV = 75f;
    public float maxFOV = 95f;

    [Tooltip("Vitesse à partir de laquelle la FOV est max")]
    public float speedForMaxFOV = 12f;

    [Header("Smoothing")]
    [Tooltip("Plus grand = transition plus lente")]
    public float fovSmoothTime = 0.15f;

    private float currentVelocity;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = baseFOV;

        if (playerRigidbody == null)
            Debug.LogError("[DynamicFOV] Rigidbody du player non assigné !");
    }

    void Update()
    {
        if (playerRigidbody == null) return;

        // Vitesse horizontale uniquement
        Vector3 horizontalVelocity = new Vector3(
            playerRigidbody.linearVelocity.x,
            0f,
            playerRigidbody.linearVelocity.z
        );

        float speed = horizontalVelocity.magnitude;

        // Normalisation vitesse → FOV
        float t = Mathf.InverseLerp(0f, speedForMaxFOV, speed);
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, t);

        // Transition douce
        cam.fieldOfView = Mathf.SmoothDamp(
            cam.fieldOfView,
            targetFOV,
            ref currentVelocity,
            fovSmoothTime
        );
    }
}