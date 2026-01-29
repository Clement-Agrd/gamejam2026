using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(FirstPersonMovement))]
public class PassThruWallAbility : MonoBehaviour
{
    public string passThruTag = "traversableWall";

    [Header("Detection settings")]
    public Vector3 boxHalfExtents = new Vector3(0.5f, 1f, 0.5f); // Taille de la box autour du joueur
    public LayerMask wallLayer; // Optionnel, pour filtrer les murs si besoin

    private Collider playerCollider;
    private FirstPersonMovement player;

    private HashSet<Collider> ignoredColliders = new HashSet<Collider>();

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
        player = GetComponent<FirstPersonMovement>();
        if (player == null)
            Debug.LogError("PassThruWallAbility requires FirstPersonMovement on the same GameObject!");
    }

    void FixedUpdate()
    {
        if (!player.canPassThruWall)
        {
            RestoreAllCollisions();
            return;
        }

        DetectWallsAndIgnoreCollisions();
        CleanupIgnoredColliders();
    }

    void DetectWallsAndIgnoreCollisions()
    {
        // On détecte tous les colliders proches dans la box
        Collider[] hits = Physics.OverlapBox(transform.position, boxHalfExtents, transform.rotation, wallLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(passThruTag) && !ignoredColliders.Contains(hit))
            {
                Physics.IgnoreCollision(playerCollider, hit, true);
                ignoredColliders.Add(hit);
            }
        }
    }

    void CleanupIgnoredColliders()
    {
        // On réactive les collisions si le mur n'est plus proche
        var temp = new HashSet<Collider>(ignoredColliders);
        foreach (var col in temp)
        {
            if (col == null) 
            {
                ignoredColliders.Remove(col);
                continue;
            }

            // Vérifie si le mur est encore dans la zone
            if (!Physics.CheckBox(transform.position, boxHalfExtents, transform.rotation, wallLayer))
            {
                Physics.IgnoreCollision(playerCollider, col, false);
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

    // Pour visualiser la box dans l'éditeur
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}
