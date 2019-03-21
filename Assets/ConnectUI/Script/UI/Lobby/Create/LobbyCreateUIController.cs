using commands;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUIController : AbstractUIController
{
	// Dropdown field containing possible GameNames
	public Dropdown dropdown;
	// The Name of this Lobby to be displayed to other users
	public InputField lobbyNameText;
	// The password of this Lobby
	public InputField passwordText;
	// The Usercount of this Lobby
	public Text maxUsercountText;

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		// Invisible Events
		eventManager.AddListener(this as IEventListener, "commands.LoginCommand");
		eventManager.AddListener(this as IEventListener, "commands.LobbyCreateCommand");
		eventManager.AddListener(this as IEventListener, "commands.LobbyJoinCommand");
		eventManager.AddListener(this as IEventListener, "commands.LogoutCommand");
	}

	// EVENTHANDLER METHODS

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		errorText.text = "";
		base.HandleEvent(evt);
		if (evt.GetData() is CommonCommand)
		{
			CommonCommand command = (CommonCommand)evt.GetData();
			if (command.Success)
			{
				SetVisible(false);
			}
			else if (command is LobbyCreateCommand)
			{
				errorText.text = command.Message;
			}
		}
		if (evt.GetData() is LoginCommand)
		{
			LoginCommand loginCommand = (LoginCommand)evt.GetData();
			if (loginCommand.Success)
			{
				dropdown.ClearOptions();
				dropdown.AddOptions(loginCommand.GameList);
				Debug.Log("Set to " + loginCommand.GameList[0]);
			}
		}
		return false;
	}

	// UTILITY METHODS

	/// <summary>
	/// Resets any UI Elements to their default state
	/// </summary>
	private void ResetUI()
	{
		errorText.text = "";
		lobbyNameText.text = "";
		passwordText.text = "";
		maxUsercountText.text = "12";
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

	// UI INPUT METHODS

	/// <summary>
	/// Changes the MaxPlayerCount Textvalue
	/// </summary>
	/// <param name="changeAmount">The amount to change</param>
	public void ChangeMaxPlayerValue(int changeAmount)
	{
		int maxUserCount;
		if (!int.TryParse(maxUsercountText.text, out maxUserCount))
		{
			errorText.text = "Max usercount needs to be a number";
			return;
		}
		
		if (maxUserCount + changeAmount <= 0)
		{
			errorText.text = "Usercount can't be < 1";
			maxUsercountText.text = 1.ToString();
		}
		else 
		{
			errorText.text = "";
			maxUserCount += changeAmount;
			maxUsercountText.text = maxUserCount.ToString();
		}
	}

	/// <summary>
	/// Called as the user presses the Create Button.
	/// Sends a LobbyCreateCommand to the server.
	/// </summary>
	public void OnLobbyCreateClick()
	{
		if (lobbyNameText.text.Equals(""))
		{
			errorText.text = "Lobby name needs to be set";
			return;
		}
		int maxUserCount;
		if (!int.TryParse(maxUsercountText.text, out maxUserCount))
		{
			errorText.text = "Max usercount needs to be a number";
			return;
		}

		NetworkClient.SendDataTCP("{\"type\":\"LobbyCreateCommand\"," +
			"\"lobbyName\":\"" + lobbyNameText.text + "\"," +
			"\"password\":\"" + Sha256(passwordText.text) + "\"," +
			"\"maxUserCount\":" + maxUserCount + "," +
			"\"gameName\":\"Tower Defense\"}");
	}

	/// <summary>
	/// Called as the user presses the Back Button.
	/// Hides this UI element
	/// </summary>
	public void OnBackButtonClick()
	{
		SetVisible(false);
	}
}