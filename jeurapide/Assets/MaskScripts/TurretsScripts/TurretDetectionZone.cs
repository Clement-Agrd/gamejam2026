using UnityEngine;

public class TurretDetectionZone : MonoBehaviour
{
    public Turret turret;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            turret.SetActive(true, other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            turret.SetActive(false, null);
        }
    }
}