using UnityEngine;

public class MeleeZone : MonoBehaviour
{
    public bool PlayerInRange;
    public PlayerHealth playerHealth;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            PlayerInRange = true;
            this.playerHealth = playerHealth;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            PlayerInRange = false;
            this.playerHealth = null;
        }
    }
}