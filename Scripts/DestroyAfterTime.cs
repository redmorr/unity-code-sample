using UnityEngine;


public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float time;

    private void OnEnable()
    {
        Destroy(gameObject, time);
    }
}