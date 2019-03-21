using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
	[SerializeField] private InteractableObjectUI _interactableObjectUi;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ShowUI()
	{
		_interactableObjectUi.SetVisible(true);
	}
}
