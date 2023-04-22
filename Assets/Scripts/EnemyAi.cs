using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAi : MonoBehaviour
{
    public int count;
    private NavMeshAgent nma;
    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;
    private bool hasDied = false;
    public static float MaxAllowedThrowPositionError = (0.25f + 0.5f) * 0.99f;
    public float Health = 50;
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

    GameObject fire;
    GameObject explosion;

    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        if (nma == null)
            Debug.Log("NavMeshAgent could not be found");

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
                //For Alpha, begin Chase immediately
                aiState = AIState.ChasePlayer;
                break;
            //... TODO handle other states
            case AIState.ChasePlayer:
                //Chase Player until close enough to shoot projectile
                NavMeshHit closestHit;
                Vector3 dest = nma.transform.position;
                if (NavMesh.SamplePosition(player.transform.position, out closestHit, 500f, NavMesh.AllAreas))
                    dest = (nma.transform.position - closestHit.position).normalized * 20 + closestHit.position;
                nma.SetDestination(dest);
                float distance = (player.transform.position - nma.transform.position).magnitude;
                if (distance <= 25)
                    aiState = AIState.AttackPlayerWithProjectile;
                break;
            case AIState.AttackPlayerWithProjectile:

                if (shootable) 
                {
                    Throw();
                    distance = (player.transform.position - nma.transform.position).magnitude;
                    if (distance > 30)
                        aiState = AIState.ChasePlayer;
                    //gameObject.transform.position, throwSpeed, Physics.gravity, player.transform.position, player.GetComponent<PlayerController>().playerVelocity, player.GetComponent<PlayerController>().cameraTransform.forward, MaxAllowedThrowPositionError
                }

                break;

            
            case AIState.Die:
                // Die, maybe adding up a global variable for winning condition?
                // Drop health or weapons for player?
                // Die animation
                //Debug.Log("DEAD");
                if (nma.baseOffset > 0)
                {
                    nma.enabled = false;
                }
                //enemy fall when die
                break;
        }

        if (Health <= 0f) {
            anim.SetBool("IsDie", true);
            rb.useGravity = true;
            aiState = AIState.Die;
            Die();
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
    
    public void Die()
    {
        if (!hasDied)
        {
            hasDied = true;

            if (OnEnemyKilled != null)
            {
                OnEnemyKilled();
            }

            StartCoroutine(DestroyEnemyAfterDelay(7.5f));
        }
    }

    private IEnumerator DestroyEnemyAfterDelay(float delay)
    {
        fire = transform.Find("Explosion").gameObject;
        fire.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(delay);
        fire.GetComponent<ParticleSystem>().Stop();
        explosion = transform.Find("Explosion2").gameObject;
        explosion.GetComponent<ParticleSystem>().Play();

        MeshRenderer[] rs = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer r in rs) {
            r.enabled = false;
        }

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);

    }
}


