using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ServerBrowserElementConfig", menuName = "Custom/ServerBrowserElementConfig", order = 1)]
public class ServerBrowserElementConfig : ScriptableObject {

	public ColorBlock defaultServerColorBlock = ColorBlock.defaultColorBlock;
	public ColorBlock serverAvailableColorBlock = ColorBlock.defaultColorBlock;
	public ColorBlock serverUnavailableColorBlock = ColorBlock.defaultColorBlock;
}
