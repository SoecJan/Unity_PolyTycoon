using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class handles MapPlaceables that contain more than one <see cref="SimpleMapPlaceable"/>.
/// </summary>
public abstract class ComplexMapPlaceable : SimpleMapPlaceable
{

	#region Attributes
	
	[SerializeField]
	private List<SimpleMapPlaceable> _childMapPlaceables;
	#endregion

	#region Getter & Setter
	public List<SimpleMapPlaceable> ChildMapPlaceables {
		get {
			return _childMapPlaceables;
		}
	}
	#endregion

	#region Default Methods
	// Use this for initialization
	void Awake()
	{
		if (ChildMapPlaceables == null)
			_childMapPlaceables = new List<SimpleMapPlaceable>();
		Initialize();
	}

	public override void Rotate(Vector3 axis, float rotationAmount)
	{
		base.Rotate(axis, rotationAmount);
//		foreach (SimpleMapPlaceable simpleMapPlaceable in ChildMapPlaceables)
//		{
//			Vector3 rotatedOffset = Quaternion.Euler(0, rotationAmount, 0) * simpleMapPlaceable.transform.localPosition;
//			simpleMapPlaceable.transform.localPosition = Vector3Int.RoundToInt(rotatedOffset);
////			Debug.Log("New:" + rotatedOffset);
//		}
	}

	#endregion
}

