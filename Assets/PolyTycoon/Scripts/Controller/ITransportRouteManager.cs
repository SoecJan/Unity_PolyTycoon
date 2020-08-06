using System.Collections.Generic;

public interface ITransportRouteManager
{
    NewRouteController RouteCreationController { get; }
    void RemoveTransportRoute(TransportRoute transportRoute);
    void CreateTransportRoute(TransportVehicleData transportVehicleData, List<TransportRouteElement> transportRouteElements);
    void EditTransportRoute(TransportRoute transportRoute);
    void DuplicateTransportRoute(TransportRoute transportRoute);
    List<TransportRoute> GetRoutes(PathType pathType);
}