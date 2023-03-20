using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnManager : MonoBehaviour
{
    System.Random random = new System.Random();
    public Transform[] enemySpawnPoint;
    public GameObject EnemyModel;

    void Start()
    {
        SpawnEnemy();
    }


    void SpawnEnemy()
    {
        int spawnindex = random.Next(0, enemySpawnPoint.Length - 1);
        //Instantiate(EnemyModel, enemySpawnPoint[spawnindex].transform.position, Quaternion.identity);

        // For Alpha, use NavMeshHit spawning below:
        NavMeshHit closestHit;
        Vector3 spawn = enemySpawnPoint[spawnindex].transform.position;
        if (NavMesh.SamplePosition(spawn, out closestHit, 500f, NavMesh.AllAreas))
            spawn = new Vector3(closestHit.position.x, closestHit.position.y + 20, closestHit.position.z);
        Instantiate(EnemyModel, spawn, Quaternion.identity);
    }

}