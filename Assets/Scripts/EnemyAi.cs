using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAi : MonoBehaviour
{

    public float Health = 100;
    public enum AIState
    {
        Patrol,
        AttackPlayerWithProjectile,
        ChasePlayer,
        Die
    };
    public AIState aiState;

    void Start()
    {
        aiState = AIState.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        switch (aiState)
        {
            case AIState.Patrol:
                //Patrol until player appear, than switch to Chase player
                break;
            case AIState.AttackPlayerWithProjectile:

                //Projectile to shoot player
                break;

            //... TODO handle other states
            case AIState.ChasePlayer:
                //Chase Player until close enough to shoot projectile
                break;
            case AIState.Die:
                // Die, maybe adding up a global variable for winning condition?
                // Drop health or weapons for player?
                // Die animation
                Debug.Log("DEAD");
                break;
        }

        if (Health <= 0f) {
            aiState = AIState.Die;
        }
    }
}
