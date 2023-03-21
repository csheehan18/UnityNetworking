using Riptide;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Dictionary<ushort, Player> List { get; private set; } = new Dictionary<ushort, Player>();

	public ushort Id { get; private set; }
	public string Username { get; private set; }

	private void OnDestroy()
	{
		List.Remove(Id);
	}
	public static void Spawn(ushort id, string username)
	{
		Player player = Instantiate(NetworkManager.Singleton.PlayerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<Player>();
		player.name = $"Player {id} ({(username == "" ? "Guest" : username)})";
		player.Id = id;
		player.Username = username;

		player.SendSpawn();
		List.Add(player.Id, player);
	}

	public void SendSpawn(ushort toClient)
	{
		NetworkManager.Singleton.Server.Send(GetSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.SpawnPlayer)), toClient);
	}
	/// <summary>Sends a player's info to all clients.</summary>
	private void SendSpawn()
	{
		NetworkManager.Singleton.Server.SendToAll(GetSpawnData(Message.Create(MessageSendMode.Reliable, ServerToClientId.SpawnPlayer)));
	}

	private Message GetSpawnData(Message message)
	{
		message.AddUShort(Id);
		message.AddString(Username);
		message.AddVector3(transform.position);
		return message;
	}

	[MessageHandler((ushort)ClientToServerId.PlayerName)]
	private static void PlayerName(ushort fromClientId, Message message)
	{
		Spawn(fromClientId, message.GetString());
		Debug.Log($"Spawning Player {fromClientId}");
	}

	[MessageHandler((ushort)ClientToServerId.CompareMovement)]
	private static void CompareMovement(ushort fromClientId, Message message)
	{
		Player player = List[fromClientId];
		if (message.GetVector3() !=  player.transform.position)
		{
			player.transform.position = message.GetVector3();
		}
	}

	[MessageHandler((ushort)ClientToServerId.PlayerInput)]
	private static void PlayerInput(ushort fromClientId, Message message)
	{
		Player player = List[fromClientId];
		player.GetComponent<PlayerController>().Movement(message.GetFloat(), message.GetFloat(), message.GetBool(), message.GetBool());
	}

	[MessageHandler((ushort)(ClientToServerId.PlayerRotate))]
	private static void PlayerRotate(ushort fromClientId, Message message)
	{
		Player player = List[fromClientId];
		player.GetComponent<PlayerController>().PlayerRotate(message.GetFloat());
	}
	//Add send keys to server to move player on server then add tick to compare them
}
