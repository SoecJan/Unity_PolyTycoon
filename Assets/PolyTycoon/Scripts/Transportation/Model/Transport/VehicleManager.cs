using System.Collections.Generic;
using UnityEngine;

public interface IVehicleManager
{
    List<TransportVehicleData> VehicleList { get; }
    TransportVehicle AddVehicle(TransportVehicleData transportVehicleData, Vector3 position, Vector3 eulerAngle = default(Vector3));
}

public class VehicleManager : MonoBehaviour, IVehicleManager
{
    #region Attributes

    private TransportVehicleUi _transportVehicleUi;
    [SerializeField] private List<TransportVehicleData> _vehicleList;

    #endregion

    #region Getter & Setter

    public List<TransportVehicleData> VehicleList => _vehicleList;

    private List<GameObject> InstancedVehicleList { get; set; }

    #endregion

    #region Methods

    // Use this for initialization
    void Start()
    {
        _transportVehicleUi = FindObjectOfType<TransportVehicleUi>();
        InstancedVehicleList = new List<GameObject>();
        TransportVehicle.OnClickAction += OnVehicleClick;
    }

    public TransportVehicle AddVehicle(TransportVehicleData transportVehicleData, Vector3 position, Vector3 eulerAngle = default(Vector3))
    {
        // Instantiate root gameobject
        GameObject rootGameObject = new GameObject(transportVehicleData.VehicleName);
        rootGameObject.transform.parent = transform;
        rootGameObject.transform.position = position;
        rootGameObject.transform.rotation = Quaternion.Euler(eulerAngle);
        // Instantiate model object and set as a child of the root
        Instantiate(transportVehicleData.Model, rootGameObject.transform);
        // Add & Setup Collider
        BoxCollider boxCollider = rootGameObject.AddComponent<BoxCollider>();
        boxCollider.center = transportVehicleData.ColliderInfo.center;
        boxCollider.size = transportVehicleData.ColliderInfo.size;
        // Add & Setup Mover
        RouteMover routeMover = rootGameObject.AddComponent<RouteMover>();
        routeMover.MaxSpeed = transportVehicleData.MaxSpeed;
        // Add & Setup Mover
        TransportVehicle transportVehicle = rootGameObject.AddComponent<TransportVehicle>();
        transportVehicle.VehicleName = transportVehicleData.VehicleName;
        transportVehicle.MaxSpeed = transportVehicleData.MaxSpeed;
        transportVehicle.MaxCapacity = transportVehicleData.MaxCapacity;
        transportVehicle.Sprite = transportVehicleData.Sprite;
        transportVehicle.RouteMover = routeMover;

        if (transportVehicleData.PathType.Equals(PathType.Rail))
        {
            WaypointMoverFollower _waypointMoverFollower = rootGameObject.AddComponent<WaypointMoverFollower>();
            _waypointMoverFollower.MoverTransform = Instantiate(Resources.Load<GameObject>("Prefabs/Transportation/Vehicle/Trailer/ContainerTrailer")).transform;
            _waypointMoverFollower.MoverTransform.position = position;
        }
        

        InstancedVehicleList.Add(rootGameObject);
        return transportVehicle;
    }

    private void OnVehicleClick(TransportVehicle transportVehicle)
    {
        _transportVehicleUi.DisplayedTransportVehicle = transportVehicle;
    }

    #endregion
}