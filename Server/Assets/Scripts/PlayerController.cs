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
	void Start()
    {
        controller= GetComponent<CharacterController>();
    }

	private void Update()
	{
		isGrounded = controller.isGrounded;

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
			controller.slopeLimit = 45.0f;
		}

		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	public void Movement(float x, float z, bool jump, bool sprint)
    {

		Vector3 move = transform.right * x + transform.forward * z;
		controller.Move(move * speed * Time.deltaTime);


		if (jump && isGrounded)
		{
			Debug.Log(jump);
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			controller.slopeLimit = 100.0f;
		}


		if ((controller.collisionFlags & CollisionFlags.Above) != 0)
		{
			velocity.y = -2f;
		}

		if (sprint)
		{
			speed = 2.5f;
		}
		else
		{
			speed = 1.28f;

		}
	}

	public void PlayerRotate(float y)
	{
		transform.rotation = Quaternion.Euler(0f,y, 0f);
	}
}
