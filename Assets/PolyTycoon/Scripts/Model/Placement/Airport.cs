using UnityEngine;

public class Airport : StorageContainer
{
    public const int LANDING = 0;
    public const int TAKEOFF = 1;

    private WayPoint[] _landingWayPoints;
    private WayPoint[] _takeoffWayPoints;

    void Start()
    {
        _simpleMapPlaceable._OnPlacementEvent += OnPlacement;
    }

    void OnPlacement(SimpleMapPlaceable simpleMapPlaceable)
    {
        Vector3 offset = transform.position;
        offset.y = 0f;
        float flightHeight = 10f;
        float groundHeight = 0f;
        float rampOffset = 0.7f;
        float terminalZOffset = 0.8f;
        
        // radius
        float groundRadius = 0.5f;
        float airRadius = 7.5f;
        
        _landingWayPoints = new WayPoint[3];
        _landingWayPoints[0] = new WayPoint(new Vector3(rampOffset,flightHeight,-airRadius*2), new Vector3(rampOffset,8f,-9.5f), new Vector3(rampOffset,flightHeight/2f,-airRadius), airRadius);
        _landingWayPoints[1] = new WayPoint(new Vector3(rampOffset,flightHeight/2,-airRadius), new Vector3(rampOffset,groundHeight,-5f), new Vector3(rampOffset,groundHeight,0.2f), airRadius);
        _landingWayPoints[2] = new WayPoint(new Vector3(rampOffset,groundHeight,0.2f), new Vector3(rampOffset,groundHeight,terminalZOffset), new Vector3(0f,groundHeight,terminalZOffset), groundRadius);
        
        _takeoffWayPoints = new WayPoint[3];
        _takeoffWayPoints[0] = new WayPoint(new Vector3(-rampOffset,groundHeight,0.2f), new Vector3(-rampOffset,groundHeight,terminalZOffset), new Vector3(0f,groundHeight,terminalZOffset), groundRadius);
        _takeoffWayPoints[1] = new WayPoint(new Vector3(-rampOffset,groundHeight,-0.2f), new Vector3(-rampOffset,groundHeight,-5f), new Vector3(-rampOffset,flightHeight/2f,-airRadius), airRadius);
        _takeoffWayPoints[2] = new WayPoint(new Vector3(-rampOffset,flightHeight/2f,-airRadius), new Vector3(-rampOffset,8f,-9.5f), new Vector3(-rampOffset,flightHeight,-airRadius*2), airRadius);

        foreach (WayPoint wayPoint in _landingWayPoints)
        {
            for (int i = 0; i < wayPoint.TraversalVectors.Length; i++)
            {
                wayPoint.TraversalVectors[i] = (Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * wayPoint.TraversalVectors[i]) + offset;
            }
        }
        
        foreach (WayPoint wayPoint in _takeoffWayPoints)
        {
            for (int i = 0; i < wayPoint.TraversalVectors.Length; i++)
            {
                wayPoint.TraversalVectors[i] = (Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * wayPoint.TraversalVectors[i]) + offset;
            }
        }
    }

    public WayPoint[] GetPlaneTraversalVectors(int toDirection)
    {
        // Landing
        switch (toDirection)
        {
            case LANDING:
                return _landingWayPoints;
            case TAKEOFF:
                return _takeoffWayPoints;
            default:
                Debug.LogError("Should not reach here!");
                return new WayPoint[0];   
        }
    }
}
