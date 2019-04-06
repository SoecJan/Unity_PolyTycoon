using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class keeps track of all placed objects in the game.
/// There may always only be one instance of this class.
/// Any placed buildings are supplied by <see cref="GroundPlacementController"/>.
/// Is used by <see cref="SimpleMapPlaceable"/> to check adjacent tiles for other <see cref="SimpleMapPlaceable"/>s.
/// </summary>
public class BuildingManager
{
	#region Attributes
	private static TransportRouteCreateController _routeCreateController;
	private static FactoryView _factoryView;
	private static CityView _cityView;

	private Dictionary<Vector3, SimpleMapPlaceable> placedBuildingDictionary; // A dict of all placed Object in the map. Used to avoid collisions.
	#endregion

	#region Constructors
	public BuildingManager()
	{
		placedBuildingDictionary = new Dictionary<Vector3, SimpleMapPlaceable>();
		_routeCreateController = Object.FindObjectOfType<TransportRouteCreateController>();
		_factoryView = Object.FindObjectOfType<FactoryView>();
		_cityView = Object.FindObjectOfType<CityView>();
	}
	#endregion

	#region Getter & Setter


	/// <summary>
	/// Returns a MapPlaceable at a given position.
	/// </summary>
	/// <param name="position"></param>
	/// <returns>The MapPlaceable at the given position. May be null. </returns>
	public SimpleMapPlaceable GetMapPlaceable(Vector3 position)
	{
		Vector3 positionVector = Vector3Int.FloorToInt(position) + new Vector3(0.5f, 0f, 0.5f);
		try
		{
			positionVector.y = 0f;
			return placedBuildingDictionary[positionVector];
		}
		catch (KeyNotFoundException)
		{
			return null;
		}
	}

	public SimpleMapPlaceable GetNode(Vector3 position)
	{
		Vector3 positionVector = Vector3Int.FloorToInt(position) + new Vector3(0.5f, 0f, 0.5f);
		try
		{
			positionVector.y = 0f;
			SimpleMapPlaceable simpleMapPlaceable = placedBuildingDictionary[positionVector];
			Vector3 comparedVector3 = simpleMapPlaceable.transform.position + simpleMapPlaceable.UsedCoordinates[0];
			comparedVector3.y = 0f;
			if (Vector3Int.FloorToInt(positionVector).Equals(Vector3Int.FloorToInt(comparedVector3))) return simpleMapPlaceable;
			return null;
		}
		catch (KeyNotFoundException)
		{
			return null;
		}
	}

	public bool IsPlaceable(ComplexMapPlaceable complexMapPlaceable)
	{
		foreach (SimpleMapPlaceable simpleMapPlaceable in complexMapPlaceable.ChildMapPlaceables)
		{
			if (!IsPlaceable(simpleMapPlaceable))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Checks if a MapPlaceable can be placed at it's postition without changing the buildingDictionary
	/// </summary>
	/// <param name="placedObject"></param>
	/// <returns></returns>
	public bool IsPlaceable(SimpleMapPlaceable placedObject)
	{
		foreach (Vector3 usedCoordinate in placedObject.UsedCoordinates)
		{
			Vector3 placedPosition = Vector3Int.FloorToInt(placedObject.transform.position);
			placedPosition = placedPosition + new Vector3(0.5f, 0, 0.5f);
			Vector3 occupiedSpace = placedPosition + usedCoordinate;
			occupiedSpace.y = 0f;
			if (placedBuildingDictionary.ContainsKey(occupiedSpace))
			{
				return false;
			}
		}

		return true;
	}
	#endregion

	#region Dictionary Modification
	/// <summary>
	/// Adds a ComplexMapPlaceable. Calls OnPlacement function on each ChildMapPlaceable.
	/// </summary>
	/// <param name="placedObject"></param>
	/// <returns></returns>
	public bool AddMapPlaceable(ComplexMapPlaceable placedObject)
	{
		if (!placedObject) return false;
		Debug.Log(placedObject.transform.name);
		for (int i = 0; i < placedObject.ChildMapPlaceables.Count; i++)
		{
			// Place Object and if not successful remove all placed Objects before the failed one.
			if (AddMapPlaceable(placedObject.ChildMapPlaceables[i])) continue;
			for (int removeIndex = i; removeIndex >= 0; removeIndex--)
			{
				RemoveMapPlaceable(placedObject.ChildMapPlaceables[removeIndex].transform.position);
			}
			return false;
		}
		return true;
	}


	/// <summary>
	/// Adds a MapPlaceable to the placedBuildingDictionary
	/// </summary>
	/// <param name="placedObject"></param>
	/// <returns></returns>
	public bool AddMapPlaceable(SimpleMapPlaceable placedObject)
	{
		if (!placedObject) return false;
		
		for (int i = 0; i < placedObject.UsedCoordinates.Count; i++)
		{
			Vector3 placedPosition = Vector3Int.FloorToInt(placedObject.transform.position);
			placedPosition = placedPosition + new Vector3(0.5f, 0, 0.5f);
			Vector3 occupiedSpace = placedPosition + placedObject.UsedCoordinates[i];
			occupiedSpace.y = 0f;

			if (!placedBuildingDictionary.ContainsKey(occupiedSpace))
			{
				placedBuildingDictionary.Add(occupiedSpace, placedObject);
			}
			else
			{
				// Remove all previously added entries
				for (int removeIndex = i; removeIndex > 0; removeIndex--)
				{
					Vector3 removedSpace = placedObject.transform.position + placedObject.UsedCoordinates[removeIndex];
					placedBuildingDictionary.Remove(removedSpace);
				}
				return false;
			}
		}
		placedObject.OnPlacement();
		placedObject.OnClickAction -= OnPlaceableClick;
		placedObject.OnClickAction += OnPlaceableClick;
		return true;
	}

	/// <summary>
	/// Removes a MapPlaceable at the specified position.
	/// </summary>
	/// <param name="position"></param>
	/// <returns>The removed MapPlaceable. May be null</returns>
	public SimpleMapPlaceable RemoveMapPlaceable(Vector3 position)
	{
		position.y = 0f;
		SimpleMapPlaceable mapPlaceable = placedBuildingDictionary[position];
		if (mapPlaceable)
		{
			foreach (Vector3 usedCoordinate in mapPlaceable.UsedCoordinates)
			{
				Vector3 occupiedSpace = position + usedCoordinate;
				if (!placedBuildingDictionary.Remove(occupiedSpace))
				{
					Debug.LogError("Position was already empty. " + occupiedSpace.ToString());
				}
				else
				{
					Object.Destroy(mapPlaceable.gameObject);
				}
			}

			return mapPlaceable;
		}
		return null;
	}

	private void OnPlaceableClick(SimpleMapPlaceable mapPlaceable)
	{
		Debug.Log("Placeable Clicked");
		if (_routeCreateController && _routeCreateController.VisibleObject.activeSelf)
		{
			if (mapPlaceable is CityBuilding)
			{
				mapPlaceable = (SimpleMapPlaceable)((CityBuilding)mapPlaceable).CityPlaceable.MainBuilding;
			}
			_routeCreateController.RouteElementController.OnTransportStationClick((PathFindingNode)mapPlaceable);
			return;
		}

		if (mapPlaceable is Factory)
		{
			_factoryView.Factory = (Factory)mapPlaceable;
		}
		else if (mapPlaceable is CityBuilding)
		{
			_cityView.CityBuilding = (CityBuilding)mapPlaceable;
		}
	}
	#endregion
}