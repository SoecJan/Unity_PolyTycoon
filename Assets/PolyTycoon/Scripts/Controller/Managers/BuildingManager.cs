using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class keeps track of all placed objects in the game.
/// There may always only be one instance of this class.
/// Any placed buildings are supplied by <see cref="PlacementController"/>.
/// Is used by <see cref="SimpleMapPlaceable"/> to check adjacent tiles for other <see cref="SimpleMapPlaceable"/>s.
/// </summary>
public class BuildingManager : IBuildingManager
{
    #region Attributes

    private static TransportRouteCreationView _routeCreationView;
    private static ProductProcessorView _productProcessorView;
    private static CityView _cityView;
    private static StorageContainerView _storageContainerView;
    private Dictionary<Vector3, SimpleMapPlaceable> _placedBuildingDictionary; // A dict of all placed Object in the map. Used to avoid collisions.

    #endregion

    #region Constructors

    public BuildingManager()
    {
        _placedBuildingDictionary = new Dictionary<Vector3, SimpleMapPlaceable>();
        _routeCreationView = Object.FindObjectOfType<TransportRouteCreationView>();
        _productProcessorView = Object.FindObjectOfType<ProductProcessorView>();
        _cityView = Object.FindObjectOfType<CityView>();
        _storageContainerView = Object.FindObjectOfType<StorageContainerView>();
        SimpleMapPlaceable.OnClickAction += OnPlaceableClick;
    }

    #endregion

    #region Getter & Setter

    private Vector3 TransformPosition(Vector3 position)
    {
        return Vector3Int.RoundToInt(position - (Vector3.one / 2f)); // Prevents rounding errors
    }

    public PathFindingNode GetNode(Vector3 position)
    {
        Vector3 positionVector = TransformPosition(position);
        try
        {
            PathFindingNode simpleMapPlaceable = _placedBuildingDictionary[positionVector] as PathFindingNode;
            if (simpleMapPlaceable)
            {
                Vector3 comparedVector3 = TransformPosition(simpleMapPlaceable.transform.position + simpleMapPlaceable.UsedCoordinates[0].UsedCoordinate);
                if (positionVector.Equals(comparedVector3)) return simpleMapPlaceable;
            }
            return null;
//            if (!(simpleMapPlaceable is Street))
//            Debug.Log("Found: " + simpleMapPlaceable.BuildingName + " at: " + position + " returned it? " + positionVector.Equals(comparedVector3) + " because: " + positionVector.ToString() + " != " + comparedVector3);
//            return positionVector.Equals(comparedVector3) ? simpleMapPlaceable : null;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if a MapPlaceable can be placed at it's postition without changing the buildingDictionary
    /// </summary>
    /// <returns></returns>
    public bool IsPlaceable(Vector3 position, List<NeededSpace> neededSpaces)
    {
        Vector3 placedPosition = TransformPosition(position);
        foreach (NeededSpace usedCoordinate in neededSpaces)
        {
            Vector3 occupiedSpace = placedPosition + usedCoordinate.UsedCoordinate;
            if (_placedBuildingDictionary.ContainsKey(occupiedSpace))
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Dictionary Modification

    /// <summary>
    /// Adds a MapPlaceable to the placedBuildingDictionary
    /// </summary>
    /// <param name="placedObject"></param>
    /// <returns></returns>
    public void AddMapPlaceable(SimpleMapPlaceable placedObject)
    {
        if (!placedObject) return;
        Vector3 placedPosition = TransformPosition(placedObject.transform.position);
        for (int i = 0; i < placedObject.UsedCoordinates.Count; i++)
        {
            Vector3 occupiedSpace = placedPosition + placedObject.UsedCoordinates[i].UsedCoordinate;

            if (!_placedBuildingDictionary.ContainsKey(occupiedSpace))
            {
                _placedBuildingDictionary.Add(occupiedSpace, placedObject);
//                Debug.Log("BuildingManager Added: " + placedObject.name + " at " + occupiedSpace);
            }
            else
            {
                // Remove all previously added entries
                for (int removeIndex = i; removeIndex > 0; removeIndex--)
                {
                    Vector3 removedSpace = TransformPosition(placedObject.transform.position) +
                                           placedObject.UsedCoordinates[removeIndex].UsedCoordinate;
                    _placedBuildingDictionary.Remove(removedSpace);
                }

                return;
            }
        }

//        placedObject.transform.position = placedPosition + (Vector3.one / 2f);
        placedObject.OnPlacement();
    }

    /// <summary>
    /// Removes a MapPlaceable at the specified position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>The removed MapPlaceable. May be null</returns>
    public void RemoveMapPlaceable(Vector3 position)
    {
        Vector3 placedPosition = TransformPosition(position);
        SimpleMapPlaceable mapPlaceable = _placedBuildingDictionary[placedPosition];
        if (mapPlaceable)
        {
            RemoveMapPlaceable(mapPlaceable);
        }
    }

    private void RemoveMapPlaceable(SimpleMapPlaceable mapPlaceable)
    {
        foreach (NeededSpace usedCoordinate in mapPlaceable.UsedCoordinates)
        {
            Vector3 position = TransformPosition(mapPlaceable.ThreadsafePosition);
            Vector3 occupiedSpace = position + usedCoordinate.UsedCoordinate;
            Debug.Log("Remove " + mapPlaceable.name + " at: " + occupiedSpace);
            if (!_placedBuildingDictionary.Remove(occupiedSpace))
            {
                Debug.LogError("Position was already empty. " + occupiedSpace.ToString());
            }
            else
            {
                Object.Destroy(mapPlaceable.gameObject);
            }
        }
    }

    public void OnPlaceableClick(SimpleMapPlaceable mapPlaceable) 
    {
        Debug.Log("Placeable clicked: " + mapPlaceable.name);
        if (_routeCreationView && _routeCreationView.VisibleObject.activeSelf)
        {
            CityBuilding cityBuilding = mapPlaceable as CityBuilding;
            if (cityBuilding != null)
            {
                mapPlaceable = cityBuilding.CityPlaceable().MainBuilding;
            }

            _routeCreationView.StationManager.OnTransportStationClick((PathFindingNode) mapPlaceable);
            return;
        }

        ProductProcessorBehaviour productProcessorBehaviour = mapPlaceable.gameObject.GetComponent<ProductProcessorBehaviour>();
        if (productProcessorBehaviour)
        {
            _productProcessorView.ProductProcessorBehaviour = productProcessorBehaviour;
        }
        else if (mapPlaceable is ICityBuilding)
        {
            _cityView.CityBuilding = (ICityBuilding) mapPlaceable;
        }
        else if (mapPlaceable is AbstractStorageContainer)
        {
            _storageContainerView.StorageContainer = (AbstractStorageContainer) mapPlaceable;
        }
    }

    #endregion
}