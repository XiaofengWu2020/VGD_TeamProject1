using UnityEngine;

// Include the namespace required to use Unity UI and Input System
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
	public Animator animator;
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	public float rotationSpeed;

	private Rigidbody controller;
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

	public float maxHealth = 100;
	public float playerHealth = 100;
	public HealthBar healthBar;

	private float timeSinceCrash = 0f;
	private float timeSinceHit= 0f;

	private bool audioFading = false;

	// At the start of the game..
	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		animator = GetComponent<Animator>();
		controller = GetComponent<Rigidbody>();
		playerInput = GetComponent<PlayerInput>();
		cameraTransform = Camera.main.transform;
		moveAction = playerInput.actions["Move"];
		floatAction = playerInput.actions["Float"];
		shootAction = playerInput.actions["Shoot"];
		shootable = true;
		shootCDcounter = shootCD;
		playerHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);

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
		moveUpdate();
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

	void moveUpdate()
    {
		timeSinceCrash += Time.deltaTime;
		timeSinceHit += Time.deltaTime;

		Vector2 hInput = moveAction.ReadValue<Vector2>();
		Vector2 vInput = floatAction.ReadValue<Vector2>();
		//playerVelocity = new Vector3(hInput.x, vInput.y, hInput.y);
		playerVelocity = hInput.x * 0.5f * cameraTransform.right.normalized + new Vector3(0, vInput.y, 0) + hInput.y * cameraTransform.forward.normalized;

		ParticleSystem particles = gameObject.GetComponent<ParticleSystem>();
		if (playerVelocity.magnitude >= 1f)
		{
			if (!particles.isPlaying)
			{
				particles.Play(false);
			}
		}
		else
		{
			particles.Stop(false);
		}

		Vector3 pos = transform.position;
		Vector3 windForce = new Vector3(0, 0, 0);
		AudioSource windAudio = wind.GetComponent<AudioSource>();
		if (pos.x < -800 || pos.x > 800 || pos.z < -800 || pos.z > 800 || pos.y > 30)
		{
			// out of bounds
			// start strong wind effect
			Vector3 dir = new Vector3(0, 0, 0);
			// Vector3 dir = Vector3.Normalize(transform.position - new Vector3(0, 10, 500)) * -1;
			if (pos.x < -800)
			{
				dir += new Vector3(1, 0, 0);
			}
			else if (pos.x > 800)
			{
				dir += new Vector3(-1, 0, 0);
			}
			if (pos.z < -800)
			{
				dir += new Vector3(0, 0, 1);
			}
			else if (pos.z > 800)
			{
				dir += new Vector3(0, 0, -1);
			}
			if (pos.y > 25)
			{
				dir += new Vector3(0, -1, 0);
			}
			windForce = dir * 4f;
			wind.transform.rotation = Quaternion.LookRotation(dir);
			wind.transform.position = transform.position - dir * 100;
			wind.GetComponent<ParticleSystem>().Play(false);

			if (!windAudio.isPlaying)
			{
				windAudio.volume = 1;
				windAudio.Play();
			}
			else if (windAudio.volume < 1f)
			{
				windAudio.volume += 0.01f;
			}
		}
		else
		{
			wind.GetComponent<ParticleSystem>().Stop(false);
			windAudio.volume -= 0.01f;
			if (windAudio.volume <= 0f && windAudio.isPlaying)
			{
				windAudio.Stop();
			}
		}

		controller.MovePosition(transform.position + windForce * Time.deltaTime);
		controller.AddForce(playerVelocity * speed, ForceMode.Force);

		Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y + 90, 0);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
	}
	void Update()
	{
		if (playerVelocity != Vector3.zero)
        {
			animator.SetFloat("Blend", 1);
			animator.SetBool("Moving", true);
		} else
        {
			animator.SetBool("Moving", false);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "EnemyBullet" && timeSinceHit > 1f)
		{
			playerHealth -= other.gameObject.GetComponent<Bullet>().DAMAGE;
			healthBar.SetHealth(playerHealth);
			timeSinceHit = 0f;
			print("player just got hit! Remain Health:" + playerHealth);
		}  
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Terrain" && timeSinceCrash > 2f)
		{
			playerHealth -= 10;
            healthBar.SetHealth(playerHealth);
            PlayEffects(other);
			timeSinceCrash = 0f;
		} 
		else if (other.gameObject.tag == "Enemy" && timeSinceCrash > 2f) 
		{
			playerHealth -= 10;
            healthBar.SetHealth(playerHealth);
            other.gameObject.GetComponent<EnemyAi>().Health -= 5;
			PlayEffects(other);
			timeSinceCrash = 0f;
		}
	}

	void PlayEffects(Collision col)
	{
		GameObject explosion = transform.Find("Explosion").gameObject;
		explosion.transform.position = col.contacts[0].point;
		explosion.GetComponent<ParticleSystem>().Play(false);
		explosion.GetComponent<AudioSource>().Play();
		// TODO: camera shake
	}

}
