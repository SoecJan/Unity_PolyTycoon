using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : IVehicleManager
{
    #region Attributes

    private GameObject _sceneObject;

    private TransportVehicleView _transportVehicleView;
    private TransportVehicleData[] _vehicleList;

    #endregion

    #region Getter & Setter

    public TransportVehicleData[] VehicleList => _vehicleList;

    private List<GameObject> InstancedVehicleList { get; set; }

    #endregion

    #region Methods

    // Use this for initialization
    public VehicleManager()
    {
        _sceneObject = new GameObject("VehicleManager");
        _transportVehicleView = GameObject.FindObjectOfType<TransportVehicleView>();
        _vehicleList = Resources.LoadAll<TransportVehicleData>(PathUtil.Get("TransportVehicleData"));
        InstancedVehicleList = new List<GameObject>();
        TransportVehicle.OnClickAction += OnVehicleClick;
    }

    public TransportVehicle AddVehicle(TransportVehicleData transportVehicleData, Vector3 position, Vector3 eulerAngle = default(Vector3))
    {
        // Instantiate root game object
        GameObject rootGameObject = new GameObject(transportVehicleData.VehicleName);
        rootGameObject.transform.parent = _sceneObject.transform;
        rootGameObject.transform.position = position;
        rootGameObject.transform.rotation = Quaternion.Euler(eulerAngle);
        // Instantiate model object and set as a child of the root
        GameObject.Instantiate(transportVehicleData.Model, rootGameObject.transform);
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
            _waypointMoverFollower.MoverTransform = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Transportation/Vehicle/Trailer/ContainerTrailer")).transform;
            _waypointMoverFollower.MoverTransform.position = position;
        }

        InstancedVehicleList.Add(rootGameObject);
        return transportVehicle;
    }

    private void OnVehicleClick(TransportVehicle transportVehicle)
    {
        _transportVehicleView.DisplayedTransportVehicle = transportVehicle;
    }

    #endregion
}