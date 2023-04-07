using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    System.Random random = new System.Random();
    public Transform[] enemySpawnPoint;
    public GameObject EnemyModel;
    public int enemiesPerWave = 15;
    public int enemiesToKillForNextWave = 3;
    public float timeBetweenWaves = 5f;

    public int enemiesRemaining;
    private int enemiesKilled;
    public int currentWave;
    private int healthIncrementPerWave = 50;

    void Start()
    {
        currentWave = 1;
        enemiesRemaining = enemiesPerWave;
        enemiesKilled = 0;
        SpawnWave();
    }

    void OnEnable()
    {
        EnemyAi.OnEnemyKilled += OnEnemyKilled;
    }

    void OnDisable()
    {
        EnemyAi.OnEnemyKilled -= OnEnemyKilled;
    }

    void OnEnemyKilled()
    {
        enemiesRemaining--;
        enemiesKilled++;

        if (enemiesKilled >= enemiesToKillForNextWave)
        {
            StartCoroutine(WaitAndSpawnNextWave());
        }
    }

    void SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
        }
    }

    IEnumerator WaitAndSpawnNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        currentWave++;
        enemiesRemaining = enemiesPerWave;
        enemiesKilled = 0;
        enemiesToKillForNextWave += 2; // Increment the enemies to kill for the next wave
        DestroyRemainingEnemies();
        SpawnWave();
    }

    void DestroyRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    void SpawnEnemy()
    {
        int spawnindex = random.Next(0, enemySpawnPoint.Length - 1);
        NavMeshHit closestHit;
        Vector3 spawn = enemySpawnPoint[spawnindex].transform.position;
        if (NavMesh.SamplePosition(spawn, out closestHit, 500f, NavMesh.AllAreas))
            spawn = new Vector3(closestHit.position.x, closestHit.position.y + 20, closestHit.position.z);
        GameObject spawnedEnemy = Instantiate(EnemyModel, spawn, Quaternion.identity);
        EnemyAi enemyAi = spawnedEnemy.GetComponent<EnemyAi>();
        enemyAi.Health += (currentWave - 1) * healthIncrementPerWave;
    }
}
