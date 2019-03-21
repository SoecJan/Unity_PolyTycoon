using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageElement : MonoBehaviour {

	public Text chatText;
	public RectTransform backgroundPanel;
	public float offset = 5f;

	public void SetText(string message)
	{
		chatText.text = message;
		backgroundPanel.sizeDelta = new Vector2(backgroundPanel.sizeDelta.x, chatText.preferredHeight + offset);
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}
}
