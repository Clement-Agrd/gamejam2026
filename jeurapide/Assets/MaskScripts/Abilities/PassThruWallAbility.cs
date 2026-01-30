using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(FirstPersonMovement))]
public class PassThruWallAbility : MonoBehaviour
{
    [Header("Tags")]
    public string traversableTag = "traversableWall";
    public string becameSolideTag = "becameSolide";

    [Header("Detection")]
    public Vector3 boxHalfExtents = new Vector3(0.5f, 1f, 0.5f);
    public LayerMask wallLayer;

    [Header("Visual feedback")]
    [Range(0f, 1f)] public float traversableAlpha = 0.4f;
    [Range(0f, 1f)] public float becameSolideAlpha = 0.3f;

    private Collider playerCollider;
    private FirstPersonMovement player;

    // Traversable walls
    private HashSet<Collider> ignoredColliders = new HashSet<Collider>();
    private Dictionary<Renderer, Color> traversableOriginalColors = new Dictionary<Renderer, Color>();

    // Became solid walls
    private HashSet<Collider> solidColliders = new HashSet<Collider>();
    private Dictionary<Renderer, Color> solidOriginalColors = new Dictionary<Renderer, Color>();

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
        player = GetComponent<FirstPersonMovement>();

        if (player == null)
            Debug.LogError("PassThruWallAbility requires FirstPersonMovement!");
    }

    void FixedUpdate()
    {
        if (!player.canPassThruWall)
        {
            // Ability OFF
            RestoreTraversableWalls();      // murs normaux redeviennent solides
            DetectBecameSolideWalls();      // BecameSolid deviennent traversables
        }
        else
        {
            // Ability ON
            DetectTraversableWalls();       // murs normaux deviennent traversables
            RestoreBecameSolideWalls();     // BecameSolid redeviennent solides
        }
    }

    // =========================
    // TRAVERSABLE WALLS
    // =========================

    void DetectTraversableWalls()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, boxHalfExtents, transform.rotation, wallLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(traversableTag))
                continue;

            if (!ignoredColliders.Contains(hit))
            {
                Physics.IgnoreCollision(playerCollider, hit, true); // devient traversable
                ignoredColliders.Add(hit);
                ApplyTransparency(hit, traversableAlpha, traversableOriginalColors);
            }
        }
    }

    void RestoreTraversableWalls()
    {
        foreach (var col in ignoredColliders)
        {
            if (col != null)
                Physics.IgnoreCollision(playerCollider, col, false); // redevient solide
        }

        RestoreVisuals(traversableOriginalColors);
        ignoredColliders.Clear();
    }

    // =========================
    // BECAME SOLIDE WALLS
    // =========================

    void DetectBecameSolideWalls()
    {
        Collider[] hits = Physics.OverlapBox(transform.position, boxHalfExtents, transform.rotation, wallLayer);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag(becameSolideTag))
                continue;

            if (!solidColliders.Contains(hit))
            {
                Physics.IgnoreCollision(playerCollider, hit, true); // devient traversable
                solidColliders.Add(hit);
                ApplyTransparency(hit, becameSolideAlpha, solidOriginalColors);
            }
        }
    }

    void RestoreBecameSolideWalls()
    {
        foreach (var col in solidColliders)
        {
            if (col != null)
                Physics.IgnoreCollision(playerCollider, col, false); // redevient solide
        }

        RestoreVisuals(solidOriginalColors);
        solidColliders.Clear();
    }

    // =========================
    // VISUAL HELPERS
    // =========================

    void ApplyTransparency(Collider col, float alpha, Dictionary<Renderer, Color> colorCache)
    {
        Renderer r = col.GetComponent<Renderer>();
        if (r == null) return;

        if (!colorCache.ContainsKey(r))
        {
            r.material = new Material(r.material); // clone material
            colorCache[r] = r.material.color;
        }

        Color c = r.material.color;
        c.a = alpha;
        r.material.color = c;
    }

    void RestoreVisuals(Dictionary<Renderer, Color> colorCache)
    {
        foreach (var pair in colorCache)
        {
            if (pair.Key != null)
                pair.Key.material.color = pair.Value;
        }

        colorCache.Clear();
    }

    // =========================
    // DEBUG GIZMOS
    // =========================

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}
