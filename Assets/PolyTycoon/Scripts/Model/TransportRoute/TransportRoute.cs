using System.Collections.Generic;


public class TransportRoute
{
    #region Attributes

    public static int RouteIndex = 0;

    #endregion

    #region Constructor

    public TransportRoute()
    {
        RouteIndex += 1;
        TransportRouteElements = new List<TransportRouteElement>();
        TransportVehicles = new List<TransportVehicle>();
    }

    #endregion

    #region Getter & Setter

    public List<TransportVehicle> TransportVehicles { get; set; }

    public List<TransportRouteElement> TransportRouteElements { get; set; }

    public string RouteName { get; set; }
    
    public List<Path> PathList
    {
        get
        {
            List<Path> vehiclePathList = new List<Path>();
            foreach (TransportRouteElement element in TransportRouteElements)
            {
                vehiclePathList.Add(element.Path);
            }
            return vehiclePathList;
        }
    }

    #endregion
}