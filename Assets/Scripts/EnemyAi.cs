using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAi : MonoBehaviour
{
    public int count;

    public static float MaxAllowedThrowPositionError = (0.25f + 0.5f) * 0.99f;
    public float Health = 100;
    public float throwSpeed = 6;
    private GameObject player;
    private Rigidbody rb;
    private Animator anim;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public int shootCD = 100;
    private int shootCDcounter;
    private bool shootable;
    public float shootForce = 6;
    public float upForce = 2;
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
        Time.timeScale = 1.0f;

        count = 0;
        anim = GetComponent<Animator>();
        aiState = AIState.Patrol;
        player = GameObject.Find("Air Balloon");
        shootCDcounter = shootCD;
        shootable = true;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!shootable)
        {
            shootCDcounter--;
        }


        if (shootCDcounter < 0)
        {
            shootable = true;
            shootCDcounter = shootCD;
        }
    }

    void Update()
    {
        switch (aiState)
        {
            case AIState.Patrol:
                //Patrol until player appear, than switch to Chase player
                break;
            //... TODO handle other states
            case AIState.ChasePlayer:
                //Chase Player until close enough to shoot projectile
                break;
            case AIState.AttackPlayerWithProjectile:

                if (shootable) 
                {
                    Throw();
                    //gameObject.transform.position, throwSpeed, Physics.gravity, player.transform.position, player.GetComponent<PlayerController>().playerVelocity, player.GetComponent<PlayerController>().cameraTransform.forward, MaxAllowedThrowPositionError
                }

                break;

            
            case AIState.Die:
                // Die, maybe adding up a global variable for winning condition?
                // Drop health or weapons for player?
                // Die animation
                Debug.Log("DEAD");
                //enemy fall when die
                break;
        }

        if (Health <= 0f) {
            anim.SetBool("IsDie", true);
            rb.useGravity = true;
            aiState = AIState.Die;
        }
    }

    public void Throw()
    {
        if (shootable)
        {
            shootable = false;

            Vector3 targetPoint;
            targetPoint = player.transform.position;
            Vector3 direction = targetPoint - bulletSpawnPoint.position;
            Vector3 dir = direction;
            dir.y = 0;
            Quaternion deltaRotation = Quaternion.Euler(dir * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            bullet.transform.forward = direction.normalized;

            bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);
            bullet.GetComponent<Rigidbody>().AddForce(Camera.main.transform.up * upForce, ForceMode.Impulse);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (Health <= 0f)
        {
            count += 1;
        }
    }

}
