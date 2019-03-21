using commands;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : AbstractUIController {
	// The ChatMessage scope this Chatcontroller should handle
	public string messageScope;

	// The Prefab for chatMessages
	public GameObject chatMessagePrefab;
	// The ContentTransform of the ScrollView for Chat Messages
	public RectTransform contentObjectTransform;

	// Inputfield for the user to specify a message to send
	public InputField messageInputField;
	// SessionID, if the user is part of a lobby
	private string sessionID;

	public string SessionID {
		get {
			return sessionID;
		}

		set {
			sessionID = value;
		}
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		eventManager.AddListener(this as IEventListener, "commands.ChatMessage");
	}

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		if (evt.GetData() is ChatMessage)
		{
			ChatMessage chatMessage = (ChatMessage)evt.GetData();
			if(chatMessage.Success && chatMessage.Scope.Equals(messageScope)) // Check for the Scope of the received Message
			{
				GameObject chatObject = Instantiate(chatMessagePrefab, contentObjectTransform); // Create a new ChatMessage Prefab Instance
				ChatMessageElement chatElement = chatObject.GetComponent<ChatMessageElement>(); // Get the ChatMessageElement
				chatElement.SetText(chatMessage.Sender + ": " + chatMessage.ChatMessageElement); // Set the Data

				RectTransform chatTransform = (RectTransform)chatObject.transform; // Get the tramsfprm of the ChatMessage
				chatTransform.anchoredPosition = new Vector2(chatTransform.anchoredPosition.x, -contentObjectTransform.sizeDelta.y); // Align the ChatElementTranform to the ScrollView
				contentObjectTransform.sizeDelta = new Vector2(contentObjectTransform.sizeDelta.x, contentObjectTransform.sizeDelta.y + chatElement.backgroundPanel.sizeDelta.y); // Increase the size of the Scrollview Transform
			}
		}
		return false; // Return false as this IEventListener does not consume the Event
	}

	/// <summary>
	/// UI Input that can be triggered by a Button. Sends a message to the Server. 
	/// Sends a Private Message, if Message is /whisper name message.
	/// Sends a Lonny Message, if a SessionID is specified.
	/// Sends a Global Message if none is specified
	/// </summary>
	public void SendChatMessage()
	{
		if (messageInputField.text.Contains("/whisper")) // Check if it's a private message
		{
			string[] output = messageInputField.text.Split(' ');
			if (output.Length > 3 && output[0].Equals("/whisper"))
			{
				string receiver = output[1];
				string message = output[2];
				NetworkClient.SendDataTCP("{\"type\":\"ChatMessage\",\"chatMessage\":\"" + message + "\",\"receiver\":\"" + receiver + "\"}");
				return;
			}
		}
		NetworkClient.SendDataTCP("{\"type\":\"ChatMessage\",\"chatMessage\":\"" + messageInputField.text + "\",\"sessionID\":\"" + SessionID + "\"}"); // Send lobby/global message
		messageInputField.text = ""; // Reset Inputfield
	}
}
