using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectUI : PlacedObject
{
	[SerializeField] private GameObject _visibleUiGameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetVisible(bool visible)
	{
		_visibleUiGameObject.SetActive(visible);
	}
}
