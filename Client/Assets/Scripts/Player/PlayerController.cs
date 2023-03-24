using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] Transform playerCamera = null;
	[SerializeField] float mouseSensitivity = 3.5f;
	//[SerializeField] bool lockCursor = true;
	[SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
	public CharacterController controller;
	public float speed = 6f;
	public float gravity = -9.81f;
	Vector3 velocity;
	public float jumpHeight = 3f;
	Vector2 currentMouseDelta = Vector2.zero;
	Vector2 currentMouseDeltaVelocity = Vector2.zero;
	private float cameraPitch = 0.0f;
	private float oldRotation;
	private bool jump;
	private bool sprint;
	private float x;
	private float z;


	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		oldRotation = transform.rotation.y;
	}

	void Update()
	{
		UpdatedMouseLook();
		CheckKeys();

	}

	void CheckKeys()
	{

		var isGrounded = controller.isGrounded;
		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
			controller.slopeLimit = 45.0f;
		}

			x = Input.GetAxisRaw("Horizontal");
			z = Input.GetAxisRaw("Vertical");
			Vector3 move = transform.right * x + transform.forward * z;
			controller.Move(move * speed * Time.deltaTime);


		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			jump = true;
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			controller.slopeLimit = 100.0f;
		}
		else
		{
			jump = false;
		}

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

		PlayerInput();


		if ((controller.collisionFlags & CollisionFlags.Above) != 0)
		{
			velocity.y = -2f;
		}

		if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
		{
			speed = 10f;
			sprint = true;
		}
		else
		{
			speed = 6f;
			sprint = false;

		}

	}

	void UpdatedMouseLook()
	{

		Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

		cameraPitch -= currentMouseDelta.y * mouseSensitivity;

		cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

		playerCamera.localEulerAngles = Vector3.right * cameraPitch;

		transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);

		if(oldRotation != transform.rotation.y)
		{
			oldRotation = transform.rotation.y;
			Debug.Log(transform.eulerAngles.y);
			PlayerRotate();
		}

	}

	public void CompareMovement()
	{
		Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.CompareMovement);
		message.AddVector3(transform.position);
		NetworkManager.Singleton.Client.Send(message);
	}

	public void PlayerInput()
	{
		Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.PlayerInput);
		message.AddFloat(x);
		message.AddFloat(z);
		message.AddBool(jump);
		message.AddBool(sprint);	
		NetworkManager.Singleton.Client.Send(message);
	}

	public void PlayerRotate()
	{
		Message message = Message.Create(MessageSendMode.Unreliable, ClientToServerId.PlayerRotate);
		message.AddFloat(transform.eulerAngles.y);
		NetworkManager.Singleton.Client.Send(message);
	}
}
