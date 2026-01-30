using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(FirstPersonMovement))]
public class PassThruWallAbility : MonoBehaviour
{
    public string passThruTag = "traversableWall";

    [Header("Detection settings")]
    public Vector3 boxHalfExtents = new Vector3(0.5f, 1f, 0.5f);
    public LayerMask wallLayer;

    [Header("Visual feedback")]
    [Range(0f, 1f)]
    public float transparentAlpha = 0.4f;

    private Collider playerCollider;
    private FirstPersonMovement player;

    private HashSet<Collider> ignoredColliders = new HashSet<Collider>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
        player = GetComponent<FirstPersonMovement>();
    }

    void FixedUpdate()
    {
        if (!player.canPassThruWall)
        {
            RestoreAllCollisions();
            RestoreAllVisuals();
            return;
        }

        DetectWallsAndIgnoreCollisions();
        CleanupIgnoredColliders();
    }

    void DetectWallsAndIgnoreCollisions()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, boxHalfExtents, transform.rotation, wallLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(passThruTag))
                continue;

            if (!ignoredColliders.Contains(hit))
            {
                Physics.IgnoreCollision(playerCollider, hit, true);
                ignoredColliders.Add(hit);
                ApplyTransparency(hit);
            }
        }
    }

    void CleanupIgnoredColliders()
    {
        var temp = new HashSet<Collider>(ignoredColliders);

        foreach (var col in temp)
        {
            if (col == null)
            {
                ignoredColliders.Remove(col);
                continue;
            }

            if (!Physics.CheckBox(transform.position, boxHalfExtents, transform.rotation, wallLayer))
            {
                Physics.IgnoreCollision(playerCollider, col, false);
                RestoreTransparency(col);
                ignoredColliders.Remove(col);
            }
        }
    }

    void RestoreAllCollisions()
    {
        foreach (var col in ignoredColliders)
        {
            if (col != null)
                Physics.IgnoreCollision(playerCollider, col, false);
        }
        ignoredColliders.Clear();
    }

    // ===== VISUAL FEEDBACK =====

    void ApplyTransparency(Collider col)
    {
        Renderer r = col.GetComponent<Renderer>();
        if (r == null) return;

        if (!originalColors.ContainsKey(r))
            originalColors[r] = r.material.color;

        Color c = r.material.color;
        c.a = transparentAlpha;
        r.material.color = c;
    }

    void RestoreTransparency(Collider col)
    {
        Renderer r = col.GetComponent<Renderer>();
        if (r == null) return;

        if (originalColors.TryGetValue(r, out Color original))
        {
            r.material.color = original;
            originalColors.Remove(r);
        }
    }

    void RestoreAllVisuals()
    {
        foreach (var pair in originalColors)
        {
            if (pair.Key != null)
                pair.Key.material.color = pair.Value;
        }
        originalColors.Clear();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}
