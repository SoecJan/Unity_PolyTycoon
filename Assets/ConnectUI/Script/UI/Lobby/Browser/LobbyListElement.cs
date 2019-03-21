using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListElement : MonoBehaviour {

	public Toggle backgroundToggle;
	public Text lobbyNameText;
	public Text gameNameText;
	public Text playerCountText;
	public Text adminNameText;
	private string sessionID;
	private bool hasPassword;

	public string SessionID {
		get {
			return sessionID;
		}

		set {
			sessionID = value;
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
