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
    }

    #endregion

    #region Getter & Setter

    public TransportVehicle Vehicle { get; set; }

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

//    public int Distance()
//    {
//        int sum = 0;
//        foreach (TransportRouteElement element in TransportRouteElements)
//        {
//            sum += element.Path.WayPoints.Count;
//        }
//        return sum;
//    }

    #endregion
}