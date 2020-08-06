using System;
using System.Collections;
using UnityEngine;

public class StationRadiusBehaviour : MonoBehaviour
{
    [SerializeField] private float _radius = 2.5f;
    [SerializeField] private GameObject _radiusVisualObj;
    private SphereCollider _collider;
    private StationBehaviour _stationBehaviour;
    
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            _collider.radius = value;
        }
    }

    public GameObject VisibleObj { get => _radiusVisualObj; set => _radiusVisualObj = value; }

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        SimpleMapPlaceable mapPlaceable = GetComponentInParent<SimpleMapPlaceable>();
        _stationBehaviour = GetComponentInParent<StationBehaviour>();
        mapPlaceable._OnPlacementEvent += delegate(SimpleMapPlaceable placeable) { StartCoroutine(OnPlacement()); };
    }

    private IEnumerator OnPlacement()
    {
        _collider.enabled = true;
        yield return null;
        _collider.enabled = false;
    }

    private void OnMouseEnter()
    {
        _radiusVisualObj.SetActive(true);
    }

    private void OnMouseExit()
    {
        _radiusVisualObj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        StationBehaviour stationBehaviour = other.gameObject.GetComponent<StationBehaviour>();
        if (stationBehaviour) return;
        
        GameObject target = other.gameObject;
        CityBuilding cityBuilding = target.GetComponent<CityBuilding>();
        if (cityBuilding) target = cityBuilding.CityPlaceable.gameObject;
        
        IProductEmitter productEmitter = target.GetComponent<IProductEmitter>();
        if (productEmitter != null 
            && !ReferenceEquals(productEmitter, _stationBehaviour) 
            && !_stationBehaviour.Emitters.Contains(productEmitter))
        {
            _stationBehaviour.Emitters.Add(productEmitter);
        }
        
        IProductReceiver productReceiver = target.GetComponent<IProductReceiver>();
        if (productReceiver != null 
            &&  !ReferenceEquals(productReceiver, _stationBehaviour) 
            && !_stationBehaviour.Receivers.Contains(productReceiver))
        {
            _stationBehaviour.Receivers.Add(productReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit " + other.gameObject.name);
    }
}
