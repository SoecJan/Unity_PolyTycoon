using commands;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUIController : AbstractUIController
{
	// The Field that the User can input his username
	public InputField usernameField;
	// The Fields that the user can input his passwords
	public InputField password1Field;
	public InputField password2Field;

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public override void RegisterToEventManager(EventManager eventManager)
	{
		base.RegisterToEventManager(eventManager);
		// Invisible Events
		eventManager.AddListener(this as IEventListener, "commands.RegisterCommand");
		eventManager.AddListener(this as IEventListener, "commands.LoginCommand");
	}

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public override bool HandleEvent(IEvent evt)
	{
		errorText.text = "";
		base.HandleEvent(evt);
		if (evt.GetData() is RegisterCommand || evt.GetData() is LoginCommand)  // Check that this event is a RegisterCommand
		{
			CommonCommand registerCommand = ((CommonCommand)evt.GetData());
			if (registerCommand.Success)
			{
				ResetUI(); // Reset Inputfields
				SetVisible(false); // Transition to the next UI
			}
			else
			{
				errorText.text = registerCommand.Message; // Set the error message
			}
		}
		return false;
	}

	/// <summary>
	/// Resets all fields to their default configuration.
	/// </summary>
	public void ResetUI()
	{
		usernameField.text = "";
		password1Field.text = "";
		password2Field.text = "";
		errorText.text = "";
	}

	/// <summary>
	/// Takes in a String and returns it's SHA256 Hash.
	/// </summary>
	/// <param name="randomString"></param>
	/// <returns>SHA256 Hash of the input String</returns>
	private string Sha256(string randomString)
	{
		var crypt = new System.Security.Cryptography.SHA256Managed();
		var hash = new System.Text.StringBuilder();
		byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
		foreach (byte theByte in crypto)
		{
			hash.Append(theByte.ToString("x2"));
		}
		return hash.ToString();
	}

	/// <summary>
	/// UI Event that needs to be triggered, if the user wants to register.
	/// </summary>
	public void OnRegisterClick()
	{
		if (usernameField.text.Equals(""))
		{
			errorText.text = "Username needs to be set";
			return;
		}
		if (password1Field.text.Equals("") || password2Field.text.Equals(""))
		{
			errorText.text = "Password needs to be set";
			return;
		}
		if (!password1Field.text.Equals(password2Field.text))
		{
			errorText.text = "Passwords need to be the same";
			return;
		}
		string hashedPassword = Sha256(password1Field.text);
		NetworkClient.SendDataTCP("{\"type\":\"RegisterCommand\",\"username\":\"" + usernameField.text + "\",\"password\":\"" + hashedPassword + "\"}");
	}
}