using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles MapPlaceables that contain more than one <see cref="SimpleMapPlaceable"/>.
/// </summary>
public class ComplexMapPlaceable : MapPlaceable
{
	[SerializeField] private List<SimpleMapPlaceable> _childMapPlaceables;

	public List<SimpleMapPlaceable> ChildMapPlaceables => _childMapPlaceables;

	// Use this for initialization
	void Awake()
	{
		if (ChildMapPlaceables == null)
			_childMapPlaceables = new List<SimpleMapPlaceable>();
	}

	public override void Rotate(Vector3 axis, float rotationAmount)
	{
		foreach(SimpleMapPlaceable childMapPlaceable in _childMapPlaceables)
		{
			if (childMapPlaceable is PathFindingTarget)
			{
				childMapPlaceable.Rotate(axis, rotationAmount);
			}
			else if (childMapPlaceable is PathFindingConnector)
			{
				childMapPlaceable.transform.localPosition = Quaternion.AngleAxis(rotationAmount, Vector3.up) * childMapPlaceable.transform.localPosition;
			}
			
		}
	}
}

