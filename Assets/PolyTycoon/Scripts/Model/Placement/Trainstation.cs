using UnityEngine;

public class Trainstation : AbstractStorageContainer
{
    [SerializeField] private Rail _accessRail;

    /// <summary>
    /// Draws the UsedCoordinates for debugging
    /// </summary>
    void OnDrawGizmos()
    {
        foreach (NeededSpace coordinate in UsedCoordinates)
        {
            Gizmos.color = coordinate.TerrainType == TerrainGenerator.TerrainType.Coast ? Color.blue : Color.yellow;
            Gizmos.DrawSphere(gameObject.transform.position + coordinate.UsedCoordinate, 0.5f);
        }
    }
    
    public Rail AccessRail => _accessRail;
}