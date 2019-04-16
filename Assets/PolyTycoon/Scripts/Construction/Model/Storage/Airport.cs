using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airport : Warehouse
{
    public const int LANDING = 0;
    public const int TAKEOFF = 1;

    private WayPoint[] _landingWayPoints;
    private WayPoint[] _takeoffWayPoints;

    public override void OnPlacement()
    {
        base.OnPlacement();
        Vector3 offset = transform.position;
        offset.y = 0f;
        
        _landingWayPoints = new WayPoint[3];
        _landingWayPoints[0] = new WayPoint(new Vector3(0.7f,10f,-15f), new Vector3(0.7f,8f,-9.5f), new Vector3(0.7f,5f,-7.5f), 7.5f);
        _landingWayPoints[1] = new WayPoint(new Vector3(0.7f,5f,-7.5f), new Vector3(0.7f,0f,-5f), new Vector3(0.7f,0f,0.2f), 7.5f);
        _landingWayPoints[2] = new WayPoint(new Vector3(0.7f,0f,0.2f), new Vector3(0.7f,0f,0.8f), new Vector3(0f,0f,0.8f), 0.5f);
        
        _takeoffWayPoints = new WayPoint[3];
        _takeoffWayPoints[0] = new WayPoint(new Vector3(-0.7f,0f,0.2f), new Vector3(-0.7f,0f,0.8f), new Vector3(-0f,0f,0.8f), 0.5f);
        _takeoffWayPoints[1] = new WayPoint(new Vector3(-0.7f,0f,-0.2f), new Vector3(-0.7f,0f,-5f), new Vector3(-0.7f,5f,-7.5f), 7.5f);
        _takeoffWayPoints[2] = new WayPoint(new Vector3(-0.7f,5f,-7.5f), new Vector3(-0.7f,8f,-9.5f), new Vector3(-0.7f,10f,-15f), 7.5f);

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
                break;
            case TAKEOFF:
                return _takeoffWayPoints;
            default:
                Debug.LogError("Should not reach here!");
                return new WayPoint[0];   
        }
    }
}
