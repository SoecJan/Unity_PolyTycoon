using System.Collections.Generic;
using UnityEngine;

public interface IComplexMapPlaceable
{
	List<SimpleMapPlaceable> ChildMapPlaceables { get; }
}

/// <summary>
/// This class handles MapPlaceables that contain more than one <see cref="SimpleMapPlaceable"/>.
/// </summary>
public class ComplexMapPlaceable : MapPlaceable, IComplexMapPlaceable
{
	[SerializeField] private List<SimpleMapPlaceable> _childMapPlaceables;

	public List<SimpleMapPlaceable> ChildMapPlaceables => _childMapPlaceables;

	void Awake()
	{
		if (ChildMapPlaceables == null)
			_childMapPlaceables = new List<SimpleMapPlaceable>();
	}

	public override void Rotate(Vector3 axis, float rotationAmount)
	{
		foreach(SimpleMapPlaceable childMapPlaceable in _childMapPlaceables)
		{
			switch (childMapPlaceable)
			{
				case PathFindingTarget _:
					childMapPlaceable.Rotate(axis, rotationAmount);
					break;
				case PathFindingConnector _:
					childMapPlaceable.transform.localPosition =
						Quaternion.AngleAxis(rotationAmount, Vector3.up) * childMapPlaceable.transform.localPosition;
					break;
			}
		}
	}
}

