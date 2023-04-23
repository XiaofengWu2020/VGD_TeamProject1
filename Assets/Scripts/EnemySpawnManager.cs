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
    public int totalWave;
    private int healthIncrementPerWave = 50;

    public EnemyRemain enemyRemain;
    public EnemyRemain enemyToKill;
    public WaveBar waveNow;

    public WinLose winState;

    void Start()
    {
        currentWave = 1;
        totalWave = 4;
        enemiesRemaining = enemiesPerWave;
        enemiesKilled = 0;
        enemyRemain.EnemyLeft(enemiesRemaining);
        enemyToKill.EnemyToKill(enemiesToKillForNextWave - enemiesKilled);
        waveNow.SetMaxWave(totalWave);
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
        enemyRemain.EnemyLeft(enemiesRemaining);
        enemyToKill.EnemyToKill(enemiesToKillForNextWave - enemiesKilled);

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
        waveNow.SetWave(currentWave);
        enemiesRemaining = enemiesPerWave;
        enemiesKilled = 0;
        enemiesToKillForNextWave += 2; // Increment the enemies to kill for the next wave
        enemyRemain.EnemyLeft(enemiesRemaining);
        enemyToKill.EnemyToKill(enemiesToKillForNextWave - enemiesKilled);
        DestroyRemainingEnemies();
        SpawnWave();
    }

    void DestroyRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            // Destroy(enemy);
            enemy.GetComponent<EnemyAi>().Despawn();
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

    void Update()
    {
        if (currentWave == 4 && enemiesKilled >= enemiesToKillForNextWave)
        winState.winShow();
    }
}
