using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileSpawner : MonoBehaviour
{
    [FormerlySerializedAs("projectilePrefab")] public DummyProjectile dummyProjectilePrefab;
    public float speed;
    
    [Button]
    private void SpawnProjectile()
    {
        DummyProjectile proj = Instantiate(dummyProjectilePrefab, transform.position, transform.rotation);
        proj.Speed = speed;
    }
}