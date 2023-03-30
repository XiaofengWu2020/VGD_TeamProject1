using UnityEngine;

// Include the namespace required to use Unity UI and Input System
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
	public Animator animator;
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	public float rotationSpeed;

	private CharacterController controller;
	private PlayerInput playerInput;
	public Vector3 playerVelocity;
	public Transform cameraTransform;
	private GameObject wind;

	private InputAction moveAction;
	private InputAction floatAction;
	private InputAction shootAction;


	public Transform bulletSpawnPoint;
	public GameObject bulletPrefab;
	public float shootForce = 6;
	public float upForce = 2;
	public int shootCD = 50;
	private int shootCDcounter = 50;
	public bool shootable;
	private float shootOffset = 0.1f;

	public float playerHealth = 100;

	private float timeSinceCrash = 0f;
	private float timeSinceHit= 0f;


	// At the start of the game..
	void Awake()
	{
		animator = GetComponent<Animator>();
		controller = GetComponent<CharacterController>();
		playerInput = GetComponent<PlayerInput>();
		cameraTransform = Camera.main.transform;
		moveAction = playerInput.actions["Move"];
		floatAction = playerInput.actions["Float"];
		shootAction = playerInput.actions["Shoot"];
		shootable = true;
		shootCDcounter = shootCD;

		wind = cameraTransform.Find("Wind").gameObject;
	}

    private void OnEnable()
    {
		shootAction.performed += _ => ShootGun();

		
    }

	private void OnDisable()
	{
		shootAction.performed -= _ => ShootGun();
	}

	private void ShootGun()
    {
		if (shootable)
		{
			shootable = false;
			Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;

			Vector3 targetPoint;
			if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
			{
				targetPoint = hit.point;
			}
			else
			{
				targetPoint = ray.GetPoint(75);
			}
			Vector3 direction = targetPoint - bulletSpawnPoint.position;

			var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
			bullet.transform.forward = direction.normalized;

			bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);
			bullet.GetComponent<Rigidbody>().AddForce(Camera.main.transform.up * upForce, ForceMode.Impulse);
			bullet.GetComponent<Rigidbody>().AddForce(Camera.main.transform.right * shootOffset, ForceMode.Impulse);
		}
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
		timeSinceCrash += Time.deltaTime;
		timeSinceHit += Time.deltaTime;

		Vector2 hInput = moveAction.ReadValue<Vector2>();
		Vector2 vInput = floatAction.ReadValue<Vector2>();
		//playerVelocity = new Vector3(hInput.x, vInput.y, hInput.y);
		playerVelocity = hInput.x * 0.5f * cameraTransform.right.normalized + new Vector3(0, vInput.y, 0)+ hInput.y * cameraTransform.forward.normalized;

		Vector3 pos = transform.position;
		Vector3 windForce = new Vector3(0, 0, 0);
		if (pos.x < -800 || pos.x > 800 || pos.z < -800 || pos.z > 800 || pos.y > 50) {
			// out of bounds
			// start strong wind effect
			Vector3 dir = Vector3.Normalize(transform.position - new Vector3(0, 10, 500)) * -1;
			windForce = dir * 2.5f;
			wind.transform.rotation = Quaternion.LookRotation(dir);
			wind.transform.position = transform.position - dir * 100;
			wind.GetComponent<ParticleSystem>().Play();
			if (!wind.GetComponent<AudioSource>().isPlaying) {
				wind.GetComponent<AudioSource>().Play();
			}
		} else {
			wind.GetComponent<ParticleSystem>().Stop();
			wind.GetComponent<AudioSource>().Stop();
		}

		controller.Move((playerVelocity * speed + windForce) * Time.deltaTime);
		if (playerVelocity != Vector3.zero)
        {
			animator.SetFloat("Blend", 1);
			animator.SetBool("Moving", true);
		} else
        {
			animator.SetBool("Moving", false);
		}

		Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y + 90, 0);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "EnemyBullet" && timeSinceHit > 1f)
		{
			playerHealth -= other.gameObject.GetComponent<Bullet>().DAMAGE;
			timeSinceHit = 0f;
			print("player just got hit! Remain Health:" + playerHealth);
		}  
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Terrain" && timeSinceCrash > 2f)
		{
			playerHealth -= 10;
			PlayEffects(other);
			timeSinceCrash = 0f;
		} 
		else if (other.gameObject.tag == "Enemy" && timeSinceCrash > 2f) 
		{
			playerHealth -= 10;
			other.gameObject.GetComponent<EnemyAi>().Health -= 5;
			PlayEffects(other);
			timeSinceCrash = 0f;
		}
	}

	void PlayEffects(Collision col)
	{
		GameObject explosion = transform.Find("Explosion").gameObject;
		explosion.transform.position = col.contacts[0].point;
		explosion.GetComponent<ParticleSystem>().Play();
		explosion.GetComponent<AudioSource>().Play();
		// TODO: camera shake
	}

}
