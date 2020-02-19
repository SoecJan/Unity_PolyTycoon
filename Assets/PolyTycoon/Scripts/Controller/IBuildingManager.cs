using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface describes the functionality for a class that holds the reference to all buildings that were placed by the player.
/// </summary>
public interface IBuildingManager
{
    PathFindingNode GetNode(Vector3 position);

    void OnPlaceableClick(SimpleMapPlaceable mapPlaceable);

    /// <summary>
    /// Checks if a MapPlaceable can be placed at it's postition without changing the buildingDictionary
    /// </summary>
    /// <returns></returns>
    bool IsPlaceable(Vector3 position, List<NeededSpace> neededSpaces);

    /// <summary>
    /// Adds a MapPlaceable to the placedBuildingDictionary
    /// </summary>
    /// <param name="placedObject"></param>
    /// <returns></returns>
    void AddMapPlaceable(SimpleMapPlaceable placedObject);

    /// <summary>
    /// Removes a MapPlaceable at the specified position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>The removed MapPlaceable. May be null</returns>
    void RemoveMapPlaceable(Vector3 position);
}