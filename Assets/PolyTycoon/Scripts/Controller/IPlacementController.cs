using System.Collections.Generic;
using UnityEngine;

public interface IPlacementController
{
    BuildingData PlaceableObjectPrefab { get; set; }
    bool PlaceObject(SimpleMapPlaceable simpleMapPlaceable);
    bool PlaceObject(ComplexMapPlaceable complexMapPlaceable);
    bool IsPlaceable(Vector3 position, List<NeededSpace> neededSpaces);
    Vector3 MoveObject(Vector3 targetPosition, Vector3 currentPosition, Vector3 offset);
}