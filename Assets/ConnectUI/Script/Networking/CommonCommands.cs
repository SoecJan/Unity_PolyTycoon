using commands.model;
using events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace events 
{ // Collection of all Clientside Events

	[Serializable]
	public class CustomEvent : IEvent
	{
		public object GetData()
		{
			return this;
		}

		public string GetName()
		{
			return this.GetType().ToString();
		}
	}

	[Serializable]
	class ConnectEvent : CustomEvent
	{
		private String ipAdress;
		private int port;

		public string IpAdress {
			get {
				return ipAdress;
			}

			set {
				ipAdress = value;
			}
		}

		public int Port {
			get {
				return port;
			}

			set {
				port = value;
			}
		}
	}

	[Serializable]
	class DisconnectEvent : CustomEvent
	{ }

	[Serializable]
	class ServerAddEvent : IEvent
	{
		private string serverName;
		private int tcpPort;
		private String possibleIpList;

		public string ServerName {
			get {
				return serverName;
			}

			set {
				serverName = value;
			}
		}

		public int TcpPort {
			get {
				return tcpPort;
			}

			set {
				tcpPort = value;
			}
		}

		public string PossibleIpList {
			get {
				return possibleIpList;
			}

			set {
				possibleIpList = value;
			}
		}

		public object GetData()
		{
			return this;
		}

		public string GetName()
		{
			return this.GetType().ToString();
		}
	}

	[Serializable]
	class ServerRemoveEvent : IEvent
	{
		private ServerBrowserElement serverBrowserElement;

		public ServerRemoveEvent(ServerBrowserElement serverBrowserElement)
		{
			this.serverBrowserElement = serverBrowserElement;
		}

		public ServerBrowserElement ServerBrowserElement {
			get {
				return serverBrowserElement;
			}

			set {
				serverBrowserElement = value;
			}
		}

		public object GetData()
		{
			return this;
		}

		public string GetName()
		{
			return this.GetType().ToString();
		}
	}
}

namespace commands 
{ // Collection of all Serverside Events

	[Serializable]
	public class CommonCommand : CustomEvent {
		private String type;
		private bool success;
		private String message;

		public string Type {
			get {
				return type;
			}

			set {
				type = value;
			}
		}

		public bool Success {
			get {
				return success;
			}

			set {
				success = value;
			}
		}

		public string Message {
			get {
				return message;
			}

			set {
				message = value;
			}
		}
	}

	// .Help
	[Serializable]
	class Help : CommonCommand {
		private String[] commands;

		public string[] Commands {
			get {
				return commands;
			}

			set {
				commands = value;
			}
		}
	}

	[Serializable]
	public class ServerSearchCommand : CommonCommand
	{
		private string serverName;
		private int tcpPort;
		private List<String> possibleIpList;

		public ServerSearchCommand(string serverName, int tcpPort, string ip)
		{ 
			this.possibleIpList = new List<string>();
			this.serverName = serverName;
			this.tcpPort = tcpPort;
			this.possibleIpList.Add(ip);
		}

		public string ServerName {
			get {
				return serverName;
			}

			set {
				serverName = value;
			}
		}

		public int TcpPort {
			get {
				return tcpPort;
			}

			set {
				tcpPort = value;
			}
		}

		public List<string> PossibleIpList {
			get {
				return possibleIpList;
			}

			set {
				possibleIpList = value;
			}
		}
	}

	// .user
	[Serializable]
	class RegisterCommand : CommonCommand {
		private String username;
		private String password;

		public string Username {
			get {
				return username;
			}

			set {
				username = value;
			}
		}

		public string Password {
			get {
				return password;
			}

			set {
				password = value;
			}
		}
	}

	[Serializable]
	class LoginCommand : CommonCommand {
		private String username;
		private String password;
		private List<String> gameList;

		public string Username {
			get {
				return username;
			}

			set {
				username = value;
			}
		}

		public string Password {
			get {
				return password;
			}

			set {
				password = value;
			}
		}

		public List<string> GameList {
			get {
				return gameList;
			}

			set {
				gameList = value;
			}
		}
	}

	[Serializable]
	class LogoutCommand : CommonCommand {}

	[Serializable]
	class DeleteCommand : CommonCommand {
		private String username;
		private String password;

		public string Username {
			get {
				return username;
			}

			set {
				username = value;
			}
		}

		public string Password {
			get {
				return password;
			}

			set {
				password = value;
			}
		}
	}

	//.chat
	[Serializable]
	class ChatMessage : CommonCommand {
		private String chatMessageElement;
		private String sender;
		private String scope;		

		public string Sender {
			get {
				return sender;
			}

			set {
				sender = value;
			}
		}

		public string Scope {
			get {
				return scope;
			}

			set {
				scope = value;
			}
		}

		public string ChatMessageElement {
			get {
				return chatMessageElement;
			}

			set {
				chatMessageElement = value;
			}
		}
	}

	// .lobby
	[Serializable]
	class LobbyCreateCommand : CommonCommand {
		private List<User> userList;
		private String lobbyName;
		private String password;
		private String session;
		private int maxUserCount;
		private String gameName;

		public string LobbyName {
			get {
				return lobbyName;
			}

			set {
				lobbyName = value;
			}
		}

		public string Password {
			get {
				return password;
			}

			set {
				password = value;
			}
		}

		public int MaxUserCount {
			get {
				return maxUserCount;
			}

			set {
				maxUserCount = value;
			}
		}

		public string GameName {
			get {
				return gameName;
			}

			set {
				gameName = value;
			}
		}

		public string Session {
			get {
				return session;
			}

			set {
				session = value;
			}
		}

		public List<User> UserList {
			get {
				return userList;
			}

			set {
				userList = value;
			}
		}
	}

	[Serializable]
	class LobbyJoinCommand : CommonCommand {
		private String sessionID;
		private String password;
		private List<User> userList;
		private String gameName;

		public string SessionID {
			get {
				return sessionID;
			}

			set {
				sessionID = value;
			}
		}

		public string Password {
			get {
				return password;
			}

			set {
				password = value;
			}
		}

		public List<User> UserList {
			get {
				return userList;
			}

			set {
				userList = value;
			}
		}

		public string GameName {
			get {
				return gameName;
			}

			set {
				gameName = value;
			}
		}
	}

	[Serializable]
	class LobbyLeaveCommand : CommonCommand {
		private String lobbySessionId;

		public string LobbySessionId {
			get {
				return lobbySessionId;
			}

			set {
				lobbySessionId = value;
			}
		}
	}

	[Serializable]
	class LobbyKickCommand : CommonCommand {
		private String lobbySessionId;
		private String kickedUsername;

		public string LobbySessionId {
			get {
				return lobbySessionId;
			}

			set {
				lobbySessionId = value;
			}
		}

		public string KickedUsername {
			get {
				return kickedUsername;
			}

			set {
				kickedUsername = value;
			}
		}
	}

	[Serializable]
	class LobbyListUpdate : CommonCommand {
		private List<NetworkLobby> lobbies;

		public List<NetworkLobby> Lobbies {
			get {
				return lobbies;
			}

			set {
				lobbies = value;
			}
		}
	}

	[Serializable]
	class LobbyUserListUpdate : CommonCommand {
		private List<User> userList;

		public List<User> UserList {
			get {
				return userList;
			}

			set {
				userList = value;
			}
		}
	}

	[Serializable]
	class LobbyReadyCommand : CommonCommand {
		private String username;
		private Boolean isReady;

		public string Username {
			get {
				return username;
			}

			set {
				username = value;
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

	// .game
	[Serializable]
	class GameStartCommand : CommonCommand {
		private string sessionID;
		private string gameName;

		public string SessionID {
			get {
				return sessionID;
			}

			set {
				sessionID = value;
			}
		}

		public string GameName {
			get {
				return gameName;
			}

			set {
				gameName = value;
			}
		}
	}

	namespace model 
	{ // Collection of all Models for the command namespace
		[Serializable]
		public class NetworkLobby
		{
			private String session;
			private String lobbyName;
			private String gameName;
			private String administrator;
			private int currentPlayerCount;
			private int maxPlayerCount;
			private Boolean hasPassword;

			public string Session {
				get {
					return session;
				}

				set {
					session = value;
				}
			}

			public string LobbyName {
				get {
					return lobbyName;
				}

				set {
					lobbyName = value;
				}
			}

			public string GameName {
				get {
					return gameName;
				}

				set {
					gameName = value;
				}
			}

			public string Administrator {
				get {
					return administrator;
				}

				set {
					administrator = value;
				}
			}

			public int CurrentPlayerCount {
				get {
					return currentPlayerCount;
				}

				set {
					currentPlayerCount = value;
				}
			}

			public int MaxPlayerCount {
				get {
					return maxPlayerCount;
				}

				set {
					maxPlayerCount = value;
				}
			}

			public bool HasPassword {
				get {
					return hasPassword;
				}

				set {
					hasPassword = value;
				}
			}
		}
	}
}
