using commands;
using events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserUIController : AbstractUIController
{
	// The saved ServerData entered by a user
	private ServerBrowserData serverBrowserData;
	// Config of all Elements inside the Scrollview
	public ServerBrowserElementConfig serverBrowserElementConfig;
	// Prefab of a ListElement
	public GameObject listElementPreFab;
	// List of all ServerElements
	private List<ServerBrowserElement> instantiatedElementList;
	// The Content Transform of the ScrollView
	public RectTransform scrollViewContentTranform;
	// The ToggleGroup containing all ServerListElements 
	public ToggleGroup toggleGroup;
	// Object that needs to be visible to add a custom server
	public GameObject objectToSetVisibleOnAdd;

	public override void Init()
	{
		base.Init();
		instantiatedElementList = new List<ServerBrowserElement>();

		// Loads saved ServerData
		StateController stateController = GameObject.FindObjectOfType<StateController>();
		foreach (ScriptableObject saveData in stateController.savedObjectsList)
		{
			if (saveData is ServerBrowserData)
			{
				serverBrowserData = ((ServerBrowserData)saveData);
				break;
			}
		}
		// Adds ServerData to the ServerList
		foreach (ServerSearchCommand serverSearchAnswer in serverBrowserData.ServerSearchAnswerList)
		{
			AddElement(serverSearchAnswer);
		}
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		// Visible Events
		base.RegisterToEventManager(eventManager);
		// Invisible Events
		eventManager.AddListener(this as IEventListener, "events.ConnectEvent");
		// Other
		eventManager.AddListener(this as IEventListener, "commands.ServerSearchCommand");
		eventManager.AddListener(this as IEventListener, "events.ServerAddEvent");
		eventManager.AddListener(this as IEventListener, "events.ServerRemoveEvent");
	}

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		errorText.text = "";
		if (evt.GetData() is ServerSearchCommand)
		{
			ServerSearchCommand serverSearchAnswer = (ServerSearchCommand)evt.GetData();
			if (serverSearchAnswer.Success)
			{
				foreach (ServerBrowserElement element in instantiatedElementList)
				{
					//if (element.serverSearchAnswer.ServerName.Equals(serverSearchAnswer.ServerName))
					if (serverSearchAnswer.PossibleIpList.All(element.serverSearchAnswer.PossibleIpList.Contains))
					{
						element.serverSearchAnswer = serverSearchAnswer;
						element.elementToggle.colors = serverBrowserElementConfig.serverAvailableColorBlock;
						element.serverAvailableText.text = "Available";
						//element.serverIpAdressText.text = "";
						return false;
					}
				}
				ServerBrowserElement browserElement = AddElement(serverSearchAnswer);
				browserElement.serverAvailableText.text = "Available";
				browserElement.elementToggle.colors = serverBrowserElementConfig.serverAvailableColorBlock;
			}
		}
		if (evt.GetData() is ConnectEvent)
		{
			SetVisible(false);
		}
		if (evt.GetData() is DisconnectEvent)
		{
			SetVisible(true);
		}
		if (evt.GetData() is ServerAddEvent)
		{
			ServerAddEvent serverAddCommand = (ServerAddEvent)evt.GetData();

			ServerSearchCommand serverSearchAnswer = new ServerSearchCommand(serverAddCommand.ServerName, serverAddCommand.TcpPort, serverAddCommand.PossibleIpList);

			foreach (ServerBrowserElement element in instantiatedElementList)
			{
				if (serverSearchAnswer.PossibleIpList.All(element.serverSearchAnswer.PossibleIpList.Contains))
				{
					element.serverSearchAnswer = serverSearchAnswer;
					element.elementToggle.colors = serverBrowserElementConfig.defaultServerColorBlock;
					return false;
				}
			}
			serverBrowserData.Add(serverSearchAnswer);
			ServerBrowserElement browserElement = AddElement(serverSearchAnswer);
			browserElement.elementToggle.colors = serverBrowserElementConfig.defaultServerColorBlock;
		}
		if (evt.GetData() is ServerRemoveEvent)
		{
			ServerRemoveEvent serverRemoveCommand = (ServerRemoveEvent)evt.GetData();
			RemoveElement(serverRemoveCommand.ServerBrowserElement);
		}
		return false;
	}

	/// <summary>
	/// Adds an Element to this ServerList
	/// </summary>
	/// <param name="serverSearchAnswer"></param>
	/// <returns></returns>
	private ServerBrowserElement AddElement(ServerSearchCommand serverSearchAnswer)
	{
		GameObject instancedObject = Instantiate(listElementPreFab, scrollViewContentTranform);
		ServerBrowserElement serverBrowserElement = instancedObject.GetComponent<ServerBrowserElement>();
		serverBrowserElement.serverNameText.text = serverSearchAnswer.ServerName;
		serverBrowserElement.serverSearchAnswer = serverSearchAnswer;
		serverBrowserElement.elementToggle.group = toggleGroup;
		serverBrowserElement.serverIpAdressText.text = serverSearchAnswer.PossibleIpList[0].ToString() + ":" + serverSearchAnswer.TcpPort;
		instantiatedElementList.Add(serverBrowserElement);
		ResizeScrollView();
		return serverBrowserElement;
	}

	/// <summary>
	/// Removes an element from this ServerList
	/// </summary>
	/// <param name="serverBrowserElement"></param>
	private void RemoveElement(ServerBrowserElement serverBrowserElement)
	{
		instantiatedElementList.Remove(serverBrowserElement);
		serverBrowserData.Remove(serverBrowserElement.serverSearchAnswer);
		Destroy(serverBrowserElement.gameObject);
		ResizeScrollView();
	}

	/// <summary>
	/// Resizes the ScrollView to fit all ServerElements
	/// </summary>
	private void ResizeScrollView()
	{
		scrollViewContentTranform.sizeDelta = new Vector2(scrollViewContentTranform.sizeDelta.x, (35 * instantiatedElementList.Count) + 5);
		for (int i = 0; i < instantiatedElementList.Count; i++)
		{
			ServerBrowserElement lobbyListElement = instantiatedElementList[i];
			GameObject lobbyListObject = lobbyListElement.gameObject;
			RectTransform rectTransform = (RectTransform)lobbyListObject.transform;
			rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, -(35 * i) - 5, 0);
		}
	}

	/// <summary>
	/// Called as the user presses the Connect Button.
	/// Tries to connect to a server via TCP using the NetworkClient class
	/// </summary>
	public void OnConnectToServerClick()
	{
		Toggle toggle = toggleGroup.ActiveToggles().FirstOrDefault();
		if (toggle)
		{
			Transform toggleParent = toggle.gameObject.transform.parent;
			GameObject lobbyElementObject = toggleParent.gameObject;
			ServerBrowserElement lobbyListElement = lobbyElementObject.GetComponent<ServerBrowserElement>();

			if (NetworkClient)
			{
				int port = lobbyListElement.serverSearchAnswer.TcpPort;
				foreach (String possibleServerIP in lobbyListElement.serverSearchAnswer.PossibleIpList)
				{
					if (NetworkClient.Connect(possibleServerIP, port))
					{
						return;
					}
				}
				errorText.text = "Could not connect to " + lobbyListElement.serverSearchAnswer.ServerName;
			}
		}
		else
		{
			errorText.text = "No Server Selected";
		}
	}

	/// <summary>
	/// Called as the user presses the Refresh Button.
	/// Sends a UDP Broadcast to get answer from reachable servers.
	/// </summary>
	public void OnRefreshClick()
	{
		if (NetworkClient)
		{
			NetworkClient.SendDataUDP("{\"type\":\"ServerSearchBroadcast\"}");
			foreach (ServerBrowserElement browserElement in instantiatedElementList)
			{
				browserElement.elementToggle.colors = serverBrowserElementConfig.serverUnavailableColorBlock;
				browserElement.serverAvailableText.text = "";
			}
		}
	}

	/// <summary>
	/// Opens the UI to add Servers to the ServerList
	/// </summary>
	public void OnAddServerClick()
	{
		objectToSetVisibleOnAdd.SetActive(true);
	}

	/// <summary>
	/// Called as the user presses the Exit Button.
	/// Quits this Application
	/// </summary>
	public void OnExitClick()
	{
		Application.Quit();
	}
}
