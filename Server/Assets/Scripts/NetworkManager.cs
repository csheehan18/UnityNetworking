using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide.Utils;
using Riptide;
using System;

public enum ServerToClientId : ushort
{
	SpawnPlayer,
	ServerMovement,
}
public enum ClientToServerId : ushort
{
	PlayerName,
	CompareMovement,
	PlayerInput,
	PlayerRotate,
}

public class NetworkManager : MonoBehaviour
{
	private static NetworkManager _singleton;
	public static NetworkManager Singleton
	{
		get => _singleton;
		private set
		{
			if (_singleton == null)
				_singleton = value;
			else if (_singleton != value)
			{
				Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying object!");
				Destroy(value);
			}
		}
	}

	[SerializeField] private ushort port;
	[SerializeField] private ushort maxClientCount;
	[SerializeField] private GameObject playerPrefab;

	public GameObject PlayerPrefab => playerPrefab;

	public Server Server { get; private set; }

	private void Awake()
	{
		Singleton = this;
	}

	private void Start()
	{
		RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

		Server = new Server();
		Server.ClientConnected += NewPlayerConnected;
		Server.ClientDisconnected += PlayerLeft;

		Server.Start(port, maxClientCount);

		Application.targetFrameRate = 30;
	}

	private void OnApplicationQuit()
	{
		Server.Stop();

		Server.ClientConnected -= NewPlayerConnected;
		Server.ClientDisconnected -= PlayerLeft;
	}

	private void FixedUpdate()
	{
		Server.Update();
	}

	private void NewPlayerConnected(object sender, ServerConnectedEventArgs e)
	{
		foreach (Player player in Player.List.Values)
		{
			if (player.Id != e.Client.Id)
			{
				player.SendSpawn(e.Client.Id);
			}
		}
	}

	private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
	{
		Destroy(Player.List[e.Client.Id].gameObject);
	}
}

