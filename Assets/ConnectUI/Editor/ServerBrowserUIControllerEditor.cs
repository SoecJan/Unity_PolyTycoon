using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ServerBrowserUIController))]
public class ServerBrowserUIControllerEditor : Editor {

	public override void OnInspectorGUI()
	{
		ServerBrowserUIController serverBrowserUIController = (ServerBrowserUIController)target;

		DrawDefaultInspector();
	}
}
