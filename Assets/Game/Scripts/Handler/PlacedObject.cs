using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
	private PlacedObject[] _neighborObjects;

    // Start is called before the first frame update
    void Start()
    {
        _neighborObjects = new PlacedObject[4];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
