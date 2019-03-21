using events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractUIController : MonoBehaviour, IEventListener {

	// NetworkClient for sending data to the server
	private static NetworkClient networkClient;
	// Object that needs to be made visible or hidden on certain events
	public List<GameObject> visibilityToggleObjectList;
	// Text to display Errors to the user
	public Text errorText;

	public static NetworkClient NetworkClient {
		get {
			return networkClient;
		}

		set {
			networkClient = value;
		}
	}

	/// <summary>
	/// Needs to be called by every derived class.
	/// Gets the NetworkClient instance and registers to the DisconnectEvent.
	/// </summary>
	public virtual void Init()
	{
		if (NetworkClient == null)
		{
			NetworkClient = GameObject.FindObjectOfType<NetworkClient>();
		}
	}

	/// <summary>
	/// Gets called by a parent Controller
	/// Used to add inactive Controllers to the EventManager
	/// </summary>
	public virtual void RegisterToEventManager(EventManager eventManager)
	{
		eventManager.AddListener(this as IEventListener, "events.DisconnectEvent");
	}

	/// <summary>
	/// Sets all visibilityToggleObjets to the visible bool.
	/// </summary>
	/// <param name="visible"></param>
	protected void SetVisible(bool visible)
	{
		foreach (GameObject visbilityObject in visibilityToggleObjectList)
		{
			visbilityObject.SetActive(visible);
		}
	}

	/// <summary>
	/// Handles Events triggered by the EventManager.
	/// Holds basic functions for all AbstractUIController.
	/// </summary>
	/// <param name="evt">Event of the EventManager</param>
	/// <returns>if the event has been consumed by this listener</returns>
	public virtual bool HandleEvent(IEvent evt)
	{
		if (evt.GetData() is DisconnectEvent)
		{
			SetVisible(false);
		}
		return false;
	}
}
