using commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHandler : MonoBehaviour, IEventListener {

	private List<User> userList;

	void Start () 
	{
		this.UserList = new List<User>();
		EventManager.instance.AddListener(this as IEventListener, "commands.LobbyJoinCommand");
		EventManager.instance.AddListener(this as IEventListener, "commands.LobbyUserListUpdate");
		EventManager.instance.AddListener(this as IEventListener, "commands.LobbyLeaveCommand");
	}

	public List<User> UserList {
		get {
			return userList;
		}

		set {
			userList = value;
		}
	}

	public bool HandleEvent(IEvent evt)
	{
		if (evt.GetData() is LobbyJoinCommand)
		{
			HandleLobbyJoinCommand((LobbyJoinCommand)evt.GetData());
		} 
		else if (evt.GetData() is LobbyUserListUpdate)
		{
			HandleLobbyUserListUpdate((LobbyUserListUpdate)evt.GetData());
		} 
		else if (evt.GetData() is LobbyLeaveCommand)
		{
			HandleLobbyLeaveCommand((LobbyLeaveCommand)evt.GetData());
		}
		return false;
	}

	private void HandleLobbyJoinCommand(LobbyJoinCommand lobbyJoinCommand)
	{
		UserList = lobbyJoinCommand.UserList;
	}

	private void HandleLobbyUserListUpdate(LobbyUserListUpdate lobbyUserListUpdate)
	{
		UserList = lobbyUserListUpdate.UserList;
	}

	private void HandleLobbyLeaveCommand(LobbyLeaveCommand lobbyLeaveCommand)
	{
		UserList.Clear();
	}
}

public class User
{
	private string username;
	private string lobbyRole;
	private string status;
	private int level;
	private bool isReady;

	public string Username {
		get {
			return username;
		}

		set {
			username = value;
		}
	}

	public string LobbyRole {
		get {
			return lobbyRole;
		}

		set {
			lobbyRole = value;
		}
	}

	public string Status {
		get {
			return status;
		}

		set {
			status = value;
		}
	}

	public int Level {
		get {
			return level;
		}

		set {
			level = value;
		}
	}

	public bool IsReady {
		get {
			return isReady;
		}

		set {
			isReady = value;
		}
	}
}
