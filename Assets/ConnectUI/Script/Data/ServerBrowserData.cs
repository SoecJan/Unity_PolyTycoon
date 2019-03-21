using commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ServerBrowserData", menuName = "Custom/ServerBrowserData", order = 1)]
public class ServerBrowserData : ScriptableObject {

	private List<ServerSearchCommand> serverSearchAnswerList;

	public List<ServerSearchCommand> ServerSearchAnswerList {
		get {
			return serverSearchAnswerList;
		}

		set {
			serverSearchAnswerList = value;
		}
	}

	public void Add(ServerSearchCommand addedServerSearchAnswer)
	{
		foreach (ServerSearchCommand serverSearchAnswer in serverSearchAnswerList)
		{
			if (serverSearchAnswer.ServerName.Equals(addedServerSearchAnswer.ServerName))
			{
				serverSearchAnswer.PossibleIpList = addedServerSearchAnswer.PossibleIpList;
				serverSearchAnswer.TcpPort = addedServerSearchAnswer.TcpPort;
				return;
			}
		}
		serverSearchAnswerList.Add(addedServerSearchAnswer);
	}

	public bool Remove(ServerSearchCommand removedServerSearchAnswer)
	{
		ServerSearchCommand saveData = null;
		foreach (ServerSearchCommand element in serverSearchAnswerList)
		{
			if (element.ServerName.Equals(removedServerSearchAnswer.ServerName))
			{
				saveData = element;
			}
		}
		return serverSearchAnswerList.Remove(saveData);
	}
}
