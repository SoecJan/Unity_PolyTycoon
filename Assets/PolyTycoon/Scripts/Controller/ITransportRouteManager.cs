using System.Collections.Generic;

public interface ITransportRouteManager
{
    void RemoveTransportRoute(TransportRoute transportRoute);
    void CreateTransportRoute(TransportVehicleData transportVehicleData, List<TransportRouteElement> transportRouteElements);
}