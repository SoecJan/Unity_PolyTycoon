using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentController : MonoBehaviour {

	public AbstractUIController[] childControllerArray;
	
	void Start () {
		RegisterChildrenToEventManager(EventManager.instance);
	}
	
	private void RegisterChildrenToEventManager(EventManager eventManager)
	{
		foreach (AbstractUIController uiController in childControllerArray)
		{
			uiController.Init();
			uiController.RegisterToEventManager(eventManager);
		}
	}
}
