using UnityEngine;

// Include the namespace required to use Unity UI and Input System
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{

	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	

	private float movementX;
	private float movementY;

	private Rigidbody rb;

	public Transform bulletSpawnPoint;
	public GameObject bulletPrefab;
	public float bulletSpeed = 6;
	private int shootCD = 100;
	private bool shootable;


	// At the start of the game..
	void Start()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();
		shootable = true;

	}

	void FixedUpdate()
	{
		// Create a Vector3 variable, and assign X and Z to feature the horizontal and vertical float variables above
		Vector3 movement = new Vector3(movementX, 0.0f, movementY);

		rb.AddForce(movement * speed);

		if (Input.GetMouseButton(0) & shootable)
		{
			shootable = false;
			var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
			bullet.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 1f, 1.5f) * bulletSpeed, ForceMode.Impulse);
		}
		shootCD--;
		if (shootCD < 0)
		{
			shootable = true;
			shootCD = 100;
		}
	}
	void Update()
	{
		if (Input.GetKeyDown("space"))
		{
			rb.AddForce(new Vector3(0.0f, 40f, 0.0f));
		}
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			rb.AddForce(new Vector3(0.0f, -40f, 0.0f));
		}
	}

	void OnTriggerEnter(Collider other)
	{
		// todo
	}

	void OnMove(InputValue value)
	{
		Vector2 v = value.Get<Vector2>();

		movementX = v.x;
		movementY = v.y;

	}

}
