using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	private static UIManager _singleton;
	public static UIManager Singleton
	{
		get => _singleton;
		private set
		{
			if (_singleton == null)
				_singleton = value;
			else if (_singleton != value)
			{
				Debug.Log($"{nameof(UIManager)} instance already exists, destroying object!");
				Destroy(value);
			}
		}
	}

	[SerializeField] private TMP_InputField usernameField;
	[SerializeField] private GameObject connectScreen;

	private void Awake()
	{
		Singleton = this;
	}

	public void ConnectClicked()
	{
		usernameField.interactable = false;
		connectScreen.SetActive(false);

		NetworkManager.Singleton.Connect();
	}

	public void BackToMain()
	{
		usernameField.interactable = true;
		connectScreen.SetActive(true);
	}

	public void SendName()
	{
		Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.PlayerName);
		message.AddString(usernameField.text);
		NetworkManager.Singleton.Client.Send(message);
	}
}

