using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float timeBetweenSpawns = 10f;

    private void Start()
    {
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (EnemyAi.Enemies.Count < 5) // Adjust the number of enemies as needed
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
