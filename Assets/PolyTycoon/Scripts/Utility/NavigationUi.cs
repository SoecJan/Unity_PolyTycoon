using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationUi : MonoBehaviour
{
    private Camera _mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _mainCamera.transform.eulerAngles.y));
    }
}
