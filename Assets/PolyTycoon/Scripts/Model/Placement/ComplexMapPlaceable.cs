using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface describes the functionality of all ComplexMapPlaceables
/// </summary>
public interface IComplexMapPlaceable
{
	/// <summary>
	/// Holds a reference to every <see cref="SimpleMapPlaceable"/> that is contained in a <see cref="IComplexMapPlaceable"/>.
	/// </summary>
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
			PathFindingNode node = childMapPlaceable.GetComponent<PathFindingNode>();
			switch (node)
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

