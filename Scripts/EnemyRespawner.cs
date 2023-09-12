using System.Collections;
using UnityEngine;

public class EnemyRespawner : MonoBehaviour
{
    [SerializeField] private AgentBase enemyPrefab;
    [SerializeField] private float respawnDelay;
    [SerializeField] private AgentBase spawnedEnemy;

    private void Start()
    {
        StartCoroutine(Spawn(0f));
    }

    private void Respawn()
    {
        StartCoroutine(Spawn(respawnDelay));
    }

    private IEnumerator Spawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spawnedEnemy)
        {
            Destroy(spawnedEnemy.gameObject);
            spawnedEnemy.OnDeath -= Respawn;
        }

        spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        spawnedEnemy.OnDeath += Respawn;
    }
}