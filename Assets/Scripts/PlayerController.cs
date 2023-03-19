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
		Vector2 hInput = moveAction.ReadValue<Vector2>();
		Vector2 vInput = floatAction.ReadValue<Vector2>();
		//playerVelocity = new Vector3(hInput.x, vInput.y, hInput.y);
		playerVelocity = hInput.x * 0.5f * cameraTransform.right.normalized + new Vector3(0, vInput.y, 0)+ hInput.y * cameraTransform.forward.normalized;
		controller.Move(playerVelocity * Time.deltaTime * speed);
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
		if (other.gameObject.tag == "EnemyBullet")
		{
			playerHealth -= other.gameObject.GetComponent<Bullet>().DAMAGE;
			print("player just got hit! Remain Health:" + playerHealth);
		}
	}

}
