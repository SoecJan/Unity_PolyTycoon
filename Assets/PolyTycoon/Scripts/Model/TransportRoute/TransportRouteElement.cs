using System.Collections.Generic;

public class TransportRouteElement
{
    #region Constructor

    public TransportRouteElement()
    {
        RouteSettings = new List<TransportRouteSetting>();
    }

    #endregion

    #region Getter & Setter

    public Path Path { get; set; }

    public PathFindingNode FromNode { get; set; }

    public PathFindingNode ToNode { get; set; }

    public List<TransportRouteSetting> RouteSettings { get; set; }

    #endregion
}