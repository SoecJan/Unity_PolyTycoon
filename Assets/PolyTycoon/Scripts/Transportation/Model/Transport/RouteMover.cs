using System.Collections.Generic;

/// <summary>
/// Extends the functionality of <see cref="WaypointMover"/> to handle a multiple paths that are driven one at a time.
/// </summary>
public class RouteMover : WaypointMover
{
    #region Attribute

    private int _pathIndex = 0;
    private List<Path> _pathList;

    #endregion

    #region Getter & Setter

    public List<Path> PathList
    {
        set
        {
            _pathList = value;
            _pathIndex = 0;
        }
    }

    public int PathIndex => _pathIndex;

    #endregion

    #region Methods

    public void MoveToNextElement()
    {
        WaypointList = _pathList[PathIndex].WayPoints;
        _pathIndex = (PathIndex + 1) % _pathList.Count;
    }

    #endregion
}