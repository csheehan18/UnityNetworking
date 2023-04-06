using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private CharacterController controller;
	public float speed = 6f;
	public float gravity = -9.81f;
	Vector3 velocity;
	public float jumpHeight = 3f;
	private bool isGrounded;
	//Changed the variables to be get and set by the player handler
	public float x { get; set; }
	public float z { get; set; }

	public float yFloat { get; set; }
	private bool doOnce;
	private bool hasJumped;

	private Vector3 oldPos;
	void Start()
    {
        controller= GetComponent<CharacterController>();
		oldPos = transform.position;
    }

	private void Update()
	{
		isGrounded = controller.isGrounded;

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
			controller.slopeLimit = 45.0f;
		}

		//Moved the movement code inside of the update function to constantly run, seems to help with only going a certain amount of times
		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move(move * speed * Time.deltaTime);

		//Moved this below the movement and seemed to fix jumping? Idk why though
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);

		if(oldPos != transform.position)
		{
			oldPos= transform.position;
			SendMovement();
			doOnce= true;
		}
		else
		{
			if (doOnce)
			{
				SendMovement();
				doOnce = false;
			}
		}
	}

	public void Movement(bool jump, bool sprint)
    {
		if (jump && isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			controller.slopeLimit = 100.0f;
			SendMovement();
		}


		if (sprint)
		{
			speed = 10f;
		}
		else
		{
			speed = 6f;

		}
	}

	public void PlayerRotate(float y)
	{
		transform.rotation = Quaternion.Euler(0f,y, 0f);
		yFloat = y;
		SendMovement();
	}

	private void SendMovement()
	{
		Message message = Message.Create(MessageSendMode.Unreliable, ServerToClientId.ServerMovement);
		message.AddUShort(this.gameObject.GetComponent<Player>().Id);
		message.AddFloat(yFloat);
		message.AddVector3(this.gameObject.transform.position);
		NetworkManager.Singleton.Server.SendToAll(message);
	}
}
