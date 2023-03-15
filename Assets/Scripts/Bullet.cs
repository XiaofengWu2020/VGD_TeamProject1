using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float DAMAGE = 50f;
    GameObject trail;
    GameObject explosion;
    private bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
     trail = gameObject.transform.GetChild(0).gameObject;   
     explosion = gameObject.transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collided) {
            return;
        }
        collided = true;
        if(other.gameObject.tag == "Enemy") {
            // decrease enemy health
            other.gameObject.GetComponent<EnemyAi>().Health -= DAMAGE;
        }
        // effects
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        trail.transform.parent = null;
        explosion.transform.parent = null;
        explosion.GetComponent<ParticleSystem>().Play();
        AudioSource[] audioSoruces = gameObject.GetComponents<AudioSource>();
        audioSoruces[1].Play();
        StartCoroutine(DestroyObjects());
    }

    IEnumerator DestroyObjects()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(trail);
        Destroy(explosion);

        yield return new WaitForSeconds(1.8f);
        Destroy(gameObject);
    }

    
}
