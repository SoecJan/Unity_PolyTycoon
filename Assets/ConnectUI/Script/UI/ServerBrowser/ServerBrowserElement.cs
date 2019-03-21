using commands;
using events;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserElement : MonoBehaviour {

	public Text serverNameText;
	public Text serverAvailableText;
	public Text serverIpAdressText;
	public Toggle elementToggle;
	public ServerSearchCommand serverSearchAnswer;

	public void OnRemoveClick()
	{
		EventManager.instance.QueueEvent(new ServerRemoveEvent(this));
	}
}
