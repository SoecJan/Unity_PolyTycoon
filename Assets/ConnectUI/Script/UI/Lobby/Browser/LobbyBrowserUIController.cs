using commands;
using events;
using commands.model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LobbyBrowserUIController : AbstractUIController
{
	// The ToggleGroup containing all LobbyListElements
	public ToggleGroup toggleGroup;
	// The Content Tranform of the Scrollview containing all LobbyListElements
	public RectTransform scrollViewContentTransform;
	// The Prefab for a LobbyListElement
	public GameObject scrollViewElementPrefab;
	// Configuration for all Elements in a Scrollview
	public ServerBrowserElementConfig serverBrowserElementConfig;
	// List of all instantiated LobbyListElements
	private List<LobbyListElement> lobbyElementList;
	// Ui for entering a Password to Join a Lobby
	public LobbyPasswordUIController lobbyPasswordUIController;
	// UI for creating lobbies
	public LobbyCreateUIController lobbyCreateUIController;

	public override void Init()
	{
		base.Init();
		lobbyElementList = new List<LobbyListElement>();
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		// Visible Events
		eventManager.AddListener(this as IEventListener, "commands.LoginCommand");
		eventManager.AddListener(this as IEventListener, "commands.LobbyLeaveCommand");
		// Invible Events
		eventManager.AddListener(this as IEventListener, "commands.LobbyJoinCommand");
		eventManager.AddListener(this as IEventListener, "commands.LobbyCreateCommand");
		eventManager.AddListener(this as IEventListener, "commands.LogoutCommand");
		// Other
		eventManager.AddListener(this as IEventListener, "commands.LobbyListUpdate");
	}

	// EVENTMANAGER METHODS

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		errorText.text = "";
		base.HandleEvent(evt);
		if (evt.GetData() is LobbyCreateCommand 
			|| evt.GetData() is LobbyJoinCommand 
			|| evt.GetData() is LogoutCommand 
			|| evt.GetData() is LoginCommand 
			|| evt.GetData() is LobbyLeaveCommand)
		{
			bool isVisible = evt.GetData() is LoginCommand || evt.GetData() is LobbyLeaveCommand;
			CommonCommand command = (CommonCommand)evt.GetData();
			if (command.Success)
			{
				SetVisible(isVisible);
				if (!isVisible)
				{
					lobbyPasswordUIController.gameObject.SetActive(false);
					lobbyCreateUIController.gameObject.SetActive(false);
				}
			}
			else
			{
				errorText.text = command.Message;
			}
		}
		else if (evt.GetData() is DisconnectEvent)
		{
			SetVisible(false);
			ResetUI();
		}
		else if (evt.GetData() is LobbyListUpdate)
		{
			LobbyListUpdate lobbyListUpdate = (LobbyListUpdate)evt.GetData();
			if (lobbyListUpdate.Success)
			{
				HandleLobbyListUpdate(lobbyListUpdate);
			}
			else
			{
				errorText.text = lobbyListUpdate.Message;
			}
		}
		return false;
	}

	// EVENT HANDLE METHODS

	/// <summary>
	/// Handles any changes to the LobbyList.
	/// </summary>
	/// <param name="lobbyListUpdate"></param>
	private void HandleLobbyListUpdate(LobbyListUpdate lobbyListUpdate)
	{
		// Delete lobbies that are not needed
		RemoveElements(lobbyListUpdate.Lobbies.Count);
		// Overwrite existing entries and create new ones if needed
		AddOrOverrideElements(lobbyListUpdate.Lobbies);
		// Make it look nice again
		ResizeScrollView();
	}

	// UTILITY METHODS

	/// <summary>
	/// Removes Elements from the LobbyList
	/// </summary>
	/// <param name="targetElementCount"></param>
	private void RemoveElements(int targetElementCount)
	{
		if (targetElementCount <= 0)
			return;

		int removeCount = lobbyElementList.Count - targetElementCount;
		for (int i = 0; i < removeCount; i++)
		{
			LobbyListElement lobbyListElement = lobbyElementList[i];
			lobbyElementList.Remove(lobbyListElement);
			Destroy(lobbyListElement.gameObject);
		}
	}

	/// <summary>
	/// Adds Elements to the LobbyList.
	/// </summary>
	/// <param name="networkLobbyList"></param>
	private void AddOrOverrideElements(List<NetworkLobby> networkLobbyList)
	{
		int currentLobbyElementCount = lobbyElementList.Count;
		int lobbyElementIndex = 0;
		foreach (NetworkLobby networkLobby in networkLobbyList)
		{
			LobbyListElement lobbyListElement = null;
			if (lobbyElementIndex < currentLobbyElementCount) // Overwrite Data
			{
				lobbyListElement = lobbyElementList[lobbyElementIndex];
				lobbyElementIndex += 1;
			}
			else // Create new Element
			{
				GameObject addedElement = Instantiate(scrollViewElementPrefab, scrollViewContentTransform);
				Toggle addedElementToggle = addedElement.GetComponentInChildren<Toggle>();
				addedElementToggle.group = toggleGroup;
				lobbyListElement = addedElement.GetComponent<LobbyListElement>();
				lobbyElementList.Add(lobbyListElement);
			}
			SetElementAttributes(lobbyListElement, networkLobby);
		}
	}

	/// <summary>
	/// Sets the UI of LobbyListElement to the NetworkLobby data
	/// </summary>
	private void SetElementAttributes(LobbyListElement lobbyListElement, NetworkLobby networkLobby)
	{
		lobbyListElement.lobbyNameText.text = networkLobby.LobbyName;
		lobbyListElement.gameNameText.text = networkLobby.GameName;
		lobbyListElement.adminNameText.text = networkLobby.Administrator;
		lobbyListElement.playerCountText.text = networkLobby.CurrentPlayerCount.ToString() + "/" + networkLobby.MaxPlayerCount.ToString();
		lobbyListElement.SessionID = networkLobby.Session;
		lobbyListElement.HasPassword = networkLobby.HasPassword;
		if (networkLobby.MaxPlayerCount > networkLobby.CurrentPlayerCount)
		{
			lobbyListElement.backgroundToggle.colors = serverBrowserElementConfig.serverAvailableColorBlock;
		}
		else
		{
			lobbyListElement.backgroundToggle.colors = serverBrowserElementConfig.defaultServerColorBlock;
		}
	}

	/// <summary>
	/// Resizes the ScrollView containing all LobbyListElements to an appropiate size.
	/// </summary>
	private void ResizeScrollView()
	{
		scrollViewContentTransform.sizeDelta = new Vector2(scrollViewContentTransform.sizeDelta.x, (60 * lobbyElementList.Count) + 5);
		for (int i = 0; i < lobbyElementList.Count; i++)
		{
			LobbyListElement lobbyListElement = lobbyElementList[i];
			GameObject lobbyListObject = lobbyListElement.gameObject;
			RectTransform rectTransform = (RectTransform)lobbyListObject.transform;
			rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, -(60 * i) - 5, 0);
		}
	}

	/// <summary>
	/// Resets any UI Fields to their default state
	/// </summary>
	private void ResetUI()
	{
		errorText.text = "";
		RemoveElements(0);
		ResizeScrollView();
	}

	// UI INPUT METHODS

	/// <summary>
	/// Called as the user presses the Refresh Button.
	/// Sends a LobbyListUpdate to the server.
	/// </summary>
	public void OnRefreshClick()
	{
		NetworkClient.SendDataTCP("{\"type\":\"LobbyListUpdate\"}");
	}

	/// <summary>
	/// Called as the user presses the Logout Button.
	/// Sends a LogoutCommand to the server.
	/// </summary>
	public void OnLogoutClick()
	{
		NetworkClient.SendDataTCP("{\"type\":\"LogoutCommand\"}");
	}

	/// <summary>
	/// Called as the user presses the Join Button.
	/// Sends a LobbyJoinCommand to the server.
	/// </summary>
	public void OnLobbyJoinClick()
	{
		Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
		if (toggle)
		{
			Transform toggleParent = toggle.gameObject.transform.parent;
			GameObject lobbyElementObject = toggleParent.gameObject;
			LobbyListElement lobbyListElement = lobbyElementObject.GetComponent<LobbyListElement>();

			if (lobbyListElement.HasPassword)
			{
				lobbyPasswordUIController.LobbyListElement = lobbyListElement;
				lobbyCreateUIController.gameObject.SetActive(false);
				lobbyPasswordUIController.gameObject.SetActive(true);
			}
			else
			{
				NetworkClient.SendDataTCP("{\"type\":\"LobbyJoinCommand\",\"lobbySessionId\":\"" + lobbyListElement.SessionID + "\",\"password\":\"\"}");
			}
		}
		else
		{
			errorText.text = "No Lobby Selected";
		}
	}

	/// <summary>
	/// Called as teh user presses the Create Button
	/// Transitions to the next UI
	/// </summary>
	public void OnCreateClick()
	{
		lobbyPasswordUIController.gameObject.SetActive(false);
		lobbyCreateUIController.gameObject.SetActive(true);
	}
}
