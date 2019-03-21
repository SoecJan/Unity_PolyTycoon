using UnityEngine;
using System.Collections;

public interface IEventListener
{
	bool HandleEvent(IEvent evt);
}

public interface IEvent
{
	string GetName();
	object GetData();
}

public class EventManager : MonoBehaviour
{
	public bool LimitQueueProcesing = false;
	public float QueueProcessTime = 0.0f;

	private static EventManager s_Instance = null;
	public static EventManager instance {
		get {
			if (s_Instance == null)
			{
				GameObject gameObject = new GameObject("EventManager");
				s_Instance = (EventManager) gameObject.AddComponent(typeof(EventManager));
			}
			return s_Instance;
		}
	}

	private Hashtable m_listenerTable = new Hashtable();
	private Queue m_eventQueue = new Queue();


	//Add a listener to the event manager that will receive any events of the supplied event name.
	public bool AddListener(IEventListener listener, string eventName)
	{
		if (listener == null || eventName == null)
		{
			Debug.Log("Event Manager: AddListener failed due to no listener or event name specified.");
			return false;
		}

		if (!m_listenerTable.ContainsKey(eventName))
			m_listenerTable.Add(eventName, new ArrayList());

		ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
		if (listenerList.Contains(listener))
		{
			Debug.Log("Event Manager: Listener: " + listener.GetType().ToString() + " is already in list for event: " + eventName);
			return false; //listener already in list
		}

		listenerList.Add(listener);
		return true;
	}

	//Remove a listener from the subscribed to event.
	public bool DetachListener(IEventListener listener, string eventName)
	{
		if (!m_listenerTable.ContainsKey(eventName))
			return false;

		ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
		if (!listenerList.Contains(listener))
			return false;

		listenerList.Remove(listener);
		return true;
	}

	//Trigger the event instantly, this should only be used in specific circumstances,
	//the QueueEvent function is usually fast enough for the vast majority of uses.
	public bool TriggerEvent(IEvent evt)
	{
		string eventName = evt.GetName();
		if (!m_listenerTable.ContainsKey(eventName))
		{
			Debug.Log("Event Manager: Event \"" + eventName + "\" triggered has no listeners!");
			return false; //No listeners for event so ignore it
		}

		ArrayList listenerList = m_listenerTable[eventName] as ArrayList;
		foreach (IEventListener listener in listenerList)
		{
			if (listener.HandleEvent(evt))
				return true; //Event consumed.
		}

		return true;
	}

	//Inserts the event into the current queue.
	public bool QueueEvent(IEvent evt)
	{
		if (!m_listenerTable.ContainsKey(evt.GetName()))
		{
			Debug.Log("EventManager: QueueEvent failed due to no listeners for event: " + evt.GetName());
			return false;
		}

		m_eventQueue.Enqueue(evt);
		return true;
	}

	//Every update cycle the queue is processed, if the queue processing is limited,
	//a maximum processing time per update can be set after which the events will have
	//to be processed next update loop.
	void Update()
	{
		float timer = 0.0f;
		while (m_eventQueue.Count > 0)
		{
			if (LimitQueueProcesing)
			{
				if (timer > QueueProcessTime)
					return;
			}

			IEvent evt = m_eventQueue.Dequeue() as IEvent;
			if (!TriggerEvent(evt))
				Debug.Log("Error when processing event: " + evt.GetName());

			if (LimitQueueProcesing)
				timer += Time.deltaTime;
		}
	}

	public void OnApplicationQuit()
	{
		m_listenerTable.Clear();
		m_eventQueue.Clear();
		s_Instance = null;
	}
}