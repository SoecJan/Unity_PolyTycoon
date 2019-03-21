using UnityEngine;
using UnityEngine.UI;
using events;
using commands;
using System.Text;

public class LoginUIController : AbstractUIController
{
	// Label to show the user the server he is connected to
	public Text serverLabel;
	// Save state of this client
	public LoginData loginData;
	// Toggle to determine if the data should be saved
	public Toggle saveToggle;
	// Inputfields for LoginData
	public InputField usernameInputField;
	public InputField passwordInputField;

	public override void Init()
	{
		base.Init();
		// Load saved LoginData
		usernameInputField.text = loginData.Username;
		passwordInputField.text = loginData.Password;
		saveToggle.isOn = loginData.IsAutoSave;
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		// Visibile Events
		eventManager.AddListener(this as IEventListener, "events.ConnectEvent");
		eventManager.AddListener(this as IEventListener, "commands.LogoutCommand");
		// InvisibleEvents
		eventManager.AddListener(this as IEventListener, "commands.RegisterCommand");
		// Other
		eventManager.AddListener(this as IEventListener, "commands.LoginCommand");
	}

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		base.HandleEvent(evt); // Handle DisconnectEvent
		if (evt.GetData() is ConnectEvent)
		{
			ConnectEvent connectCommand = ((ConnectEvent)evt.GetData());
			HandleConnectCommand(connectCommand);
		}
		else if (evt.GetData() is LoginCommand)
		{
			LoginCommand loginCommand = ((LoginCommand)evt.GetData());
			HandleLoginCommand(loginCommand);
		}
		else if (evt.GetData() is LogoutCommand)
		{
			LogoutCommand logoutCommand = ((LogoutCommand)evt.GetData());
			if (logoutCommand.Success)
			{
				SetVisible(true);
				usernameInputField.text = loginData.Username;
				passwordInputField.text = loginData.Password;
			}
		}
		else if (evt.GetData() is RegisterCommand)
		{
			RegisterCommand registerCommand = (RegisterCommand)evt.GetData();
			if (registerCommand.Success)
			{
				SetVisible(true);
			}
		}
		return false;
	}

	// HANDLE METHODS

	/// <summary>
	/// After connecting to the Server show this UI and set the serverlabel.
	/// </summary>
	/// <param name="connectCommand"></param>
	private void HandleConnectCommand(ConnectEvent connectCommand)
	{
		serverLabel.text = connectCommand.IpAdress + ":" + connectCommand.Port.ToString();
		SetVisible(true);
	}

	/// <summary>
	/// After logging in hide this UI and save LoginData
	/// </summary>
	/// <param name="loginCommand"></param>
	private void HandleLoginCommand(LoginCommand loginCommand)
	{
		if (loginCommand.Success)
		{
			SaveData();
			ResetUI();
			SetVisible(false);
		}
		else
		{
			serverLabel.text = loginCommand.Message;
		}
	}

	// UTILITY METHODS

	/// <summary>
	/// Saves the Data entered by a user
	/// </summary>
	private void SaveData()
	{
		loginData.IsAutoSave = saveToggle.isOn;
		loginData.Username = saveToggle.isOn ? usernameInputField.text : "";
		loginData.Password = saveToggle.isOn ? passwordInputField.text : "";
	}

	/// <summary>
	/// Resets InputFields to their default state
	/// </summary>
	private void ResetUI()
	{
		usernameInputField.text = "";
		passwordInputField.text = "";
	}

	private string Sha256(string originString)
	{
		var crypt = new System.Security.Cryptography.SHA256Managed();
		var hash = new System.Text.StringBuilder();
		byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(originString));
		foreach (byte theByte in crypto)
		{
			hash.Append(theByte.ToString("x2"));
		}
		return hash.ToString();
	}

	// UI METHODS

	/// <summary>
	/// Called as the user presses the Back button.
	/// Disconnects from the Server.
	/// </summary>
	public void OnBackButtonClick()
	{
		NetworkClient.Dispose();
	}

	/// <summary>
	/// Called as the user presses the Login Button.
	/// Sends a LoginCommand to the server.
	/// </summary>
	public void OnLoginButtonClick()
	{
		string hashedPassword = Sha256(passwordInputField.text);
		NetworkClient.SendDataTCP("{\"type\":\"LoginCommand\",\"username\":\"" + usernameInputField.text + "\",\"password\":\"" + hashedPassword + "\"}");
	}
}
