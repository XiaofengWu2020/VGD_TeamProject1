using UnityEngine;

// Include the namespace required to use Unity UI and Input System
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{

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
	private int shootCD = 100;
	private bool shootable;


	// At the start of the game..
	void Awake()
	{
		// Assign the Rigidbody component to our private rb variable
		controller = GetComponent<CharacterController>();
		playerInput = GetComponent<PlayerInput>();
		cameraTransform = Camera.main.transform;
		moveAction = playerInput.actions["Move"];
		floatAction = playerInput.actions["Float"];
		shootAction = playerInput.actions["Shoot"];
		shootable = true;

	}

    private void OnEnable()
    {
		if (shootable) { 
			shootAction.performed += _ => ShootGun();
		}
		
    }

	private void OnDisable()
	{
		shootAction.performed -= _ => ShootGun();
	}

	private void ShootGun()
    {
		shootable = false;
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		RaycastHit hit;

		Vector3 targetPoint;
		if(Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
			targetPoint = hit.point;
        } else
        {
			targetPoint = ray.GetPoint(75);
        }
		Vector3 direction = targetPoint - bulletSpawnPoint.position;

		var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
		bullet.transform.forward = direction.normalized;

		bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);
		bullet.GetComponent<Rigidbody>().AddForce(Camera.main.transform.up * upForce, ForceMode.Impulse);

	}

	void FixedUpdate()
	{
		shootCD--;

		if (shootCD < 0)
		{
			shootable = true;
			shootCD = 100;
		}
	}

	void Update()
	{
		Vector2 hInput = moveAction.ReadValue<Vector2>();
		Vector2 vInput = floatAction.ReadValue<Vector2>();
		//playerVelocity = new Vector3(hInput.x, vInput.y, hInput.y);
		playerVelocity = hInput.x * 0.5f * cameraTransform.right.normalized + new Vector3(0, vInput.y, 0)+ hInput.y * cameraTransform.forward.normalized;
		controller.Move(playerVelocity * Time.deltaTime * speed);

		Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y + 90, 0);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

	}

	void OnTriggerEnter(Collider other)
	{
		// todo
	}

}
