using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using Riptide.Transports;
using UnityEngine.UIElements;

public class ServerMovement : MonoBehaviour
{
	private List<Vector3> newPos = new List<Vector3>();

	public void Rotate(float rot) 
	{
		transform.rotation = Quaternion.Euler(0f, rot, 0f);
	}

	public void Movement(Vector3 pos)
	{
		newPos.Add(pos);
	}
	public void Update()
	{
		for(int i = 0; i < newPos.Count; i++)
		{
			transform.position = Vector3.Lerp(this.transform.position, newPos[i], 0.5f);
		}
	}


}
