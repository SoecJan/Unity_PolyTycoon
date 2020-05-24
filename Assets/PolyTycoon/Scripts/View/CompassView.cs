using UnityEngine;

public class CompassView : MonoBehaviour
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
