using UnityEngine;

public class InteractableDisappear : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    public LevelEnd levelEnd;


    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactDistance && Input.GetKeyDown(interactKey))
        {
            levelEnd.FinDuNiveau();
            Disappear();
        }
    }

    private void Disappear()
    {
        gameObject.SetActive(false);
        // ou Destroy(gameObject); si tu veux le supprimer définitivement
    }
}