using commands;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPasswordUIController : AbstractUIController {

	public Text titleText;
	public InputField passwordInputField;
	private LobbyListElement lobbyListElement;

	public LobbyListElement LobbyListElement {
		get {
			return lobbyListElement;
		}

		set {
			lobbyListElement = value;
			titleText.text = "Join: " + lobbyListElement.lobbyNameText.text;
		}
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		eventManager.AddListener(this as IEventListener, "commands.LobbyJoinCommand");
		eventManager.AddListener(this as IEventListener, "commands.LogoutCommand");
	}

	public override bool HandleEvent(IEvent evt)
	{
		errorText.text = "";
		base.HandleEvent(evt);
		if (evt.GetData() is LobbyJoinCommand)
		{
			LobbyJoinCommand lobbyJoinCommand = (LobbyJoinCommand)evt.GetData();
			if (lobbyJoinCommand.Success)
			{
				SetVisible(false);
			}
			else
			{
				errorText.text = lobbyJoinCommand.Message;
			}
		}
		if (evt.GetData() is LogoutCommand)
		{
			LogoutCommand logoutCommand = (LogoutCommand)evt.GetData();
			if (logoutCommand.Success)
			{
				SetVisible(false);
			}
		}
		return false;
	}

	private string Sha256(string originString)
	{
		var crypt = new System.Security.Cryptography.SHA256Managed();
		var hash = new System.Text.StringBuilder();
		byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(originString));
		foreach (byte theByte in crypto)
		{
			hash.Append(theByte.ToString("x2"));
		}
		return hash.ToString();
	}

	public void OnJoinClick()
	{
		if (passwordInputField.text.Equals("")) return;

		string hashedPassword = Sha256(passwordInputField.text);
		NetworkClient.SendDataTCP("{\"type\":\"LobbyJoinCommand\",\"lobbySessionId\":\"" + LobbyListElement.SessionID + "\",\"password\":\""+hashedPassword+"\"}");
	}

	public void OnBackClick()
	{
		SetVisible(false);
	}
}
