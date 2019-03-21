using events;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserAddUIController : AbstractUIController
{
	public InputField serverNameInputField;
	public InputField serverIpInputField;
	public InputField serverPortInputField;

	public void AddElement()
	{
		if (serverNameInputField.text.Trim().Equals(""))
		{
			errorText.text = "Name is not set";
			return;
		}
		int port;
		if (!int.TryParse(serverPortInputField.text, out port))
		{
			errorText.text = "Port is not a number";
			return;
		}

		IPAddress address;
		if (!IPAddress.TryParse(serverIpInputField.text, out address))
		{
			errorText.text = "IP is not valid";
			return;
		}

		ServerAddEvent serverAddCommand = new ServerAddEvent();
		serverAddCommand.ServerName = serverNameInputField.text;
		serverAddCommand.TcpPort = port;
		serverAddCommand.PossibleIpList = address.ToString();
		EventManager.instance.QueueEvent(serverAddCommand);
		ResetUI();
		gameObject.SetActive(false);
	}

	private void ResetUI()
	{
		serverNameInputField.text = "";
		serverIpInputField.text = "";
		serverPortInputField.text = "";
	}
}