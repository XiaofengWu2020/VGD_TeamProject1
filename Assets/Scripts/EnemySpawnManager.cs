using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Instantiate(EnemyModel, enemySpawnPoint[spawnindex].transform.position, Quaternion.identity);
    }

}