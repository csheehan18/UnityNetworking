using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

	public ushort Id { get; private set; }
	public bool IsLocal { get; private set; }

	private string username;

	public static void Spawn(ushort id, string username, Vector3 position)
	{
		Player player;
		if (id == NetworkManager.Singleton.Client.Id)
		{
			player = Instantiate(NetworkManager.Singleton.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
		}
		else
			player = Instantiate(NetworkManager.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();

		player.name = $"Player {id} ({username})";
		player.Id = id;
		player.username = username;
		list.Add(player.Id, player);
	}


	[MessageHandler((ushort)ServerToClientId.SpawnPlayer)]
	private static void SpawnPlayer(Message message)
	{
		Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
	}

	[MessageHandler((ushort)ServerToClientId.ServerMovement)]
	private static void PlayerMovement(Message message)
	{
		ushort playerId = message.GetUShort();
		if (playerId == NetworkManager.Singleton.Client.Id)
		{
			return;
		}
		if (list.TryGetValue(playerId, out Player player))
			player.GetComponent<ServerMovement>().Rotate(message.GetFloat());
			player.GetComponent<ServerMovement>().Movement(message.GetVector3());
	}

}

