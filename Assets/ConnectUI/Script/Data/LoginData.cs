using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoginData", menuName = "Custom/LoginData", order = 1)]
public class LoginData : ScriptableObject {

	private string username;
	private string password;
	private bool isAutomaticSave;

	public string Password {
		get {
			return password;
		}

		set {
			password = value;
		}
	}

	public string Username {
		get {
			return username;
		}

		set {
			username = value;
		}
	}

	public bool IsAutoSave {
		get {
			return isAutomaticSave;
		}

		set {
			isAutomaticSave = value;
		}
	}
}
