using commands;
using events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : AbstractUIController
{
	// Chatcontroller for the LobbyChat
	public ChatController chatController;
	// ToggleGroup for the Userlist
	public ToggleGroup toggleGroup;
	// Button to kick other players if you are the admin
	public Button kickButton;
	// Text of the readyButton. Determines if you are the Admin on LobbyReadyCommand.
	public Text readyButtonText;

	// The SessionID and GameName of this lobby
	private string sessionID;
	private string gameName;

	// The ScrollView Transform holding all UserListElements
	public RectTransform scrollViewContentTransform;
	// Prefab of a UserListElement
	public GameObject scrollViewElementPrefab;
	// Config for the same config on all ScrollViews
	public ServerBrowserElementConfig serverBrowserElementConfig;

	// List of all UserListElements.
	private List<LobbyUserListElement> lobbyElementList;

	public override void Init()
	{
		base.Init();
		this.lobbyElementList = new List<LobbyUserListElement>();
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		// Visible Events
		eventManager.AddListener(this as IEventListener, "commands.LobbyCreateCommand");
		eventManager.AddListener(this as IEventListener, "commands.LobbyJoinCommand");
		// Invisible Events
		eventManager.AddListener(this as IEventListener, "commands.LobbyLeaveCommand");
		// Other
		eventManager.AddListener(this as IEventListener, "commands.LobbyKickCommand");
		eventManager.AddListener(this as IEventListener, "commands.LobbyUserListUpdate");
		eventManager.AddListener(this as IEventListener, "commands.LobbyReadyCommand");
	}

	// EVENT MANAGER METHODS

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		errorText.text = "";
		base.HandleEvent(evt);
		if (evt.GetData() is LobbyJoinCommand)
		{
			ProcessLobbyJoin((LobbyJoinCommand)evt.GetData());
		}
		else if (evt.GetData() is LobbyLeaveCommand)
		{
			ProcessLobbyLeave((LobbyLeaveCommand)evt.GetData());
		}
		else if (evt.GetData() is LobbyKickCommand)
		{
			ProcessLobbyKick((LobbyKickCommand)evt.GetData());
		}
		else if (evt.GetData() is LobbyCreateCommand)
		{
			ProcessLobbyCreate((LobbyCreateCommand)evt.GetData());
		}
		else if (evt.GetData() is LobbyUserListUpdate)
		{
			ProcessUserListUpdate((LobbyUserListUpdate)evt.GetData());
		}
		else if (evt.GetData() is LobbyReadyCommand)
		{
			ProcessUserReady((LobbyReadyCommand)evt.GetData());
		}
		else if (evt.GetData() is DisconnectEvent)
		{
			SetVisible(false);
		}
		return false;
	}

	// EVENT HANDLE METHODS

	/// <summary>
	/// Processes any changes to the Userlist content
	/// </summary>
	/// <param name="lobbyUserListUpdate"></param>
	private void ProcessUserListUpdate(LobbyUserListUpdate lobbyUserListUpdate)
	{
		RemoveNotNeededElements(lobbyElementList.Count - lobbyUserListUpdate.UserList.Count);
		UpdateOrCreateListEntries(lobbyUserListUpdate.UserList);
		ResizeScrollView();
	}

	/// <summary>
	/// Processes if a user has clicked the ready button
	/// </summary>
	/// <param name="lobbyReadyCommand"></param>
	private void ProcessUserReady(LobbyReadyCommand lobbyReadyCommand)
	{
		foreach (LobbyUserListElement listElement in lobbyElementList)
		{
			if (listElement.usernameText.text.Equals(lobbyReadyCommand.Username))
			{ 
				listElement.toggle.colors = lobbyReadyCommand.IsReady ? serverBrowserElementConfig.serverAvailableColorBlock : serverBrowserElementConfig.defaultServerColorBlock;
			}
		}
	}

	/// <summary>
	/// Processes creating a lobby
	/// </summary>
	/// <param name="lobbyCreateCommand"></param>
	private void ProcessLobbyCreate(LobbyCreateCommand lobbyCreateCommand)
	{
		if (lobbyCreateCommand.Success)
		{
			SetVisible(true);
			kickButton.gameObject.SetActive(true);
			readyButtonText.text = "Start";
			SetupLobbyButtons(lobbyCreateCommand.Session, lobbyCreateCommand.GameName);
			RemoveNotNeededElements(
			lobbyElementList.Count - 
			lobbyCreateCommand.UserList.Count);
			UpdateOrCreateListEntries(lobbyCreateCommand.UserList);
			ResizeScrollView();
		}
		else
		{
			errorText.text = lobbyCreateCommand.Message;
		}
	}

	/// <summary>
	/// Processes joining a lobby
	/// </summary>
	/// <param name="lobbyJoinCommand"></param>
	private void ProcessLobbyJoin(LobbyJoinCommand lobbyJoinCommand)
	{
		if (lobbyJoinCommand.Success)
		{
			SetVisible(true);
			chatController.gameObject.SetActive(true);
			SetupLobbyButtons(lobbyJoinCommand.SessionID, lobbyJoinCommand.GameName);
			readyButtonText.text = "Ready";
			RemoveNotNeededElements(lobbyElementList.Count - lobbyJoinCommand.UserList.Count);
			UpdateOrCreateListEntries(lobbyJoinCommand.UserList);
			ResizeScrollView();
		}
		else
		{
			errorText.text = lobbyJoinCommand.Message;
		}
	}

	/// <summary>
	/// Processes leaving the lobby. 
	/// </summary>
	/// <param name="lobbyLeaveCommand"></param>
	private void ProcessLobbyLeave(LobbyLeaveCommand lobbyLeaveCommand)
	{
		if (lobbyLeaveCommand.Success)
		{
			SetVisible(false);
		}
		else
		{
			errorText.text = lobbyLeaveCommand.Message;
		}
	}

	/// <summary>
	/// Processes any LobbyKickCommand
	/// </summary>
	/// <param name="lobbyKickCommand"></param>
	private void ProcessLobbyKick(LobbyKickCommand lobbyKickCommand)
	{
		if (!lobbyKickCommand.Success)
		{
			errorText.text = lobbyKickCommand.Message;
		}
	}

	// UTILITY METHODS

	/// <summary>
	/// Sets the LobbyUserListElement UI to fit to the User Object
	/// </summary>
	/// <param name="lobbyListElement"></param>
	/// <param name="user"></param>
	private void PopulateUserListElement(LobbyUserListElement lobbyListElement, User user)
	{
		lobbyListElement.usernameText.text = user.Username;
		lobbyListElement.statusText.text = user.LobbyRole;
		lobbyListElement.levelText.text = "Level: " + user.Level;
		lobbyListElement.toggle.colors = user.IsReady ? serverBrowserElementConfig.serverAvailableColorBlock : serverBrowserElementConfig.defaultServerColorBlock;
	}

	/// <summary>
	/// Removes an amount of Users from the Userlist
	/// </summary>
	/// <param name="countElementsRemove"></param>
	private void RemoveNotNeededElements(int countElementsRemove)
	{
		if (countElementsRemove > 0) // Delete users that are not needed
		{
			for (int i = 0; i < countElementsRemove; i++)
			{
				LobbyUserListElement lobbyListElement = lobbyElementList[i];
				lobbyElementList.Remove(lobbyListElement);
				Destroy(lobbyListElement.gameObject);
			}
		}
	}

	/// <summary>
	/// Handles the change of the Userlist
	/// </summary>
	/// <param name="users"></param>
	private void UpdateOrCreateListEntries(List<User> users)
	{
		int currentLobbyElementCount = lobbyElementList.Count;
		int lobbyElementIndex = 0;
		foreach (User lobbyUser in users)
		{
			if (lobbyElementIndex < currentLobbyElementCount) // Overwrite Data
			{
				LobbyUserListElement lobbyListElement = lobbyElementList[lobbyElementIndex];
				PopulateUserListElement(lobbyListElement, lobbyUser);
				lobbyElementIndex += 1;
			}
			else // Create new Element
			{
				GameObject addedElement = Instantiate(scrollViewElementPrefab, scrollViewContentTransform);
				Toggle addedElementToggle = addedElement.GetComponentInChildren<Toggle>();
				addedElementToggle.group = toggleGroup;
				LobbyUserListElement lobbyListElement = addedElement.GetComponent<LobbyUserListElement>();
				PopulateUserListElement(lobbyListElement, lobbyUser);
				lobbyElementList.Add(lobbyListElement);
			}
		}
	}

	/// <summary>
	/// Sets the SessionID and Gamename of this Lobby
	/// </summary>
	/// <param name="sessionID"></param>
	/// <param name="gameName"></param>
	private void SetupLobbyButtons(string sessionID, string gameName)
	{
		this.sessionID = sessionID;
		this.gameName = gameName;
		chatController.SessionID = sessionID;
	}

	/// <summary>
	/// Resizes the Scrollview containing LobbyUserListElements to fit all of them.
	/// </summary>
	private void ResizeScrollView()
	{
		scrollViewContentTransform.sizeDelta = new Vector2(scrollViewContentTransform.sizeDelta.x, (40 * lobbyElementList.Count) + 5);
		for (int i = 0; i < lobbyElementList.Count; i++)
		{
			LobbyUserListElement lobbyListElement = lobbyElementList[i];
			GameObject lobbyListObject = lobbyListElement.gameObject;
			RectTransform lobbyListTransform = ((RectTransform)lobbyListObject.transform);
			lobbyListTransform.anchoredPosition = new Vector3(lobbyListTransform.anchoredPosition.x, -(40 * i) - 5, 0);
		}
	}

	// UI INPUT METHODS

	/// <summary>
	/// Called as the user presses the Ready Button.
	/// Sends a LobbyReadyommand to the server.
	/// </summary>
	public void OnReadyClick()
	{
		if (readyButtonText.text.Equals("Start"))
		{
			NetworkClient.SendDataTCP("{\"type\":\"GameStartCommand\",\"sessionID\":\"" + sessionID + "\",\"gameName\":\"" + gameName + "\"}");
		}
		else
		{
			NetworkClient.SendDataTCP("{\"type\":\"LobbyReadyCommand\",\"sessionID\":\"" + sessionID + "\"}");
		}
	}

	/// <summary>
	/// Called as the user presses the Kick Button.
	/// Sends a LobbyKickCommand to the server.
	/// </summary>
	public void OnKickCommand()
	{
		Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
		if (toggle)
		{
			Transform toggleParent = toggle.gameObject.transform.parent;
			GameObject lobbyElementObject = toggleParent.gameObject;
			LobbyUserListElement lobbyListElement = lobbyElementObject.GetComponent<LobbyUserListElement>();
			string kickedUsername = lobbyListElement.usernameText.text;
			NetworkClient.SendDataTCP("{\"type\":\"LobbyKickCommand\",\"lobbySessionId\":\"" + sessionID + "\",\"kickedUsername\":\"" + kickedUsername + "\"}");
		}
		else
		{
			Debug.Log("No User selected");
		}
	}

	/// <summary>
	/// Called as the user presses the Exit Button.
	/// Sends a LobbyLeaveCommand to the server.
	/// </summary>
	public void OnExitClick()
	{
		NetworkClient.SendDataTCP("{\"type\":\"LobbyLeaveCommand\",\"lobbySessionId\":\"" + sessionID + "\"}");
	}
}
