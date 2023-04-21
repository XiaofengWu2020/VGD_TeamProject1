using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyRemain : MonoBehaviour
{

    public Text enemyRemain;
    public Text enemyToKill;


    public void EnemyLeft(float remain)
    {
        enemyRemain.text = remain.ToString();
    }

    public void EnemyToKill(float kill)
    {
        enemyToKill.text = kill.ToString();
    }
}