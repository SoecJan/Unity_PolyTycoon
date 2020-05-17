using UnityEngine;

public interface IVehicleManager
{
    TransportVehicleData[] VehicleList { get; }
    TransportVehicle AddVehicle(TransportVehicleData transportVehicleData, Vector3 position, Vector3 eulerAngle = default(Vector3));
}