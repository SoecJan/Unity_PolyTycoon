using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using tcpTestClient.src.networking;
using commands;
using events;
using Newtonsoft.Json;

public class NetworkClient : MonoBehaviour, ITCPClientController, IEventListener
{

	private MyTCPClient tcpClient;
	private MyUdpClient udpClient;
	private InboundMessageParser inboundMessageParser;

	// Use this for initialization
	void Start()
	{
		EventManager.instance.AddListener(this as IEventListener, "commands.ConnectCommand");
		inboundMessageParser = new InboundMessageParser();
		tcpClient = new MyTCPClient();
		tcpClient.onServerConnect += OnServerConnectionEstablished;
		tcpClient.onDataReceiveCallBack += OnDataReceive;
		tcpClient.onServerDisconnect += OnServerConnectionLost;

		udpClient = new MyUdpClient();
		udpClient.OnUdpMessageReceive += OnDataReceive;
	}

	public bool Connect(String ip, int port)
	{
		tcpClient.IpAdress = ip;
		tcpClient.Port = port;
		return tcpClient.Start();
	}

	void OnApplicationQuit()
	{
		Dispose();
	}

	#region UDPClient implementation

	public void SendDataUDP(string udpData)
	{
		udpClient.Send(udpData);
	}

	#endregion

	#region ITCPClientController implementation

	public void SendDataTCP(string data)
	{
		tcpClient.SendData(data);
	}

	public void OnDataReceive(string data)
	{
		Debug.Log(data);
		CommonCommand command = inboundMessageParser.parseJSON(data);
		EventManager.instance.QueueEvent(command);
	}

	public void OnServerConnectionEstablished(string ipAdress, int port)
	{
		ConnectEvent connectCommand = new ConnectEvent();
		connectCommand.IpAdress = ipAdress;
		connectCommand.Port = port;
		EventManager.instance.QueueEvent(connectCommand);
	}

	public void OnServerConnectionLost(string reason)
	{
		DisconnectEvent disconnectCommand = new DisconnectEvent();
		EventManager.instance.QueueEvent(disconnectCommand);
		Debug.Log("Server connection lost");
		udpClient.Start();
	}

	public void Dispose()
	{
		if (tcpClient != null)
			tcpClient.Dispose();
		if (udpClient != null)
			udpClient.Close();
	}

	public bool HandleEvent(IEvent evt)
	{
		if (evt.GetData() is ConnectEvent)
		{
			udpClient.Close();
		}
		return false;
	}

	#endregion
}

class InboundMessageParser
{

	public CommonCommand parseJSON(String json)
	{
		CommonCommand command = JsonConvert.DeserializeObject<CommonCommand>(json);
		switch (command.Type)
		{

			case "Help":
				command = JsonConvert.DeserializeObject<Help>(json);
				break;
			case "ServerSearchAnswer":
				command = JsonConvert.DeserializeObject<ServerSearchCommand>(json);
				break;

			// .user
			case "LoginCommand":
				command = JsonConvert.DeserializeObject<LoginCommand>(json);
				break;
			case "LogoutCommand":
				command = JsonConvert.DeserializeObject<LogoutCommand>(json);
				break;
			case "RegisterCommand":
				command = JsonConvert.DeserializeObject<RegisterCommand>(json);
				break;
			case "DeleteCommand":
				command = JsonConvert.DeserializeObject<DeleteCommand>(json);
				break;

			// .chat
			case "ChatMessage":
				command = JsonConvert.DeserializeObject<ChatMessage>(json);
				break;

			// .lobby
			case "LobbyCreateCommand":
				command = JsonConvert.DeserializeObject<LobbyCreateCommand>(json);
				break;
			case "LobbyKickCommand":
				command = JsonConvert.DeserializeObject<LobbyKickCommand>(json);
				break;
			case "LobbyJoinCommand":
				command = JsonConvert.DeserializeObject<LobbyJoinCommand>(json);
				break;
			case "LobbyLeaveCommand":
				command = JsonConvert.DeserializeObject<LobbyLeaveCommand>(json);
				break;
			case "LobbyListUpdate":
				command = JsonConvert.DeserializeObject<LobbyListUpdate>(json);
				break;
			case "LobbyReadyCommand":
				command = JsonConvert.DeserializeObject<LobbyReadyCommand>(json);
				break;
			case "LobbyUserListUpdate":
				command = JsonConvert.DeserializeObject<LobbyUserListUpdate>(json);
				break;

			// .game
			case "GameStartCommand":
				command = JsonConvert.DeserializeObject<GameStartCommand>(json);
				break;

			// .default
			default:
				Debug.LogError("JSON Type Error: " + json);
				break;
		}

		return command;
	}
}
