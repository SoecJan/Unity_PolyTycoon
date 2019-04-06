using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(VehicleMover))]
public class Vehicle : MonoBehaviour
{
	#region Attributes
	private int _routeIndex = -1;
	private List<Path> _pathList;
	[SerializeField] private VehicleMover _mover;
	[SerializeField] private Sprite _sprite;
	#endregion

	#region Getter & Setter
	public VehicleMover Mover {
		get {
			return _mover;
		}

		set {
			_mover = value;
		}
	}

	public Sprite Sprite {
		get {
			return _sprite;
		}
	}

	public List<Path> PathList {
		get {
			return _pathList;
		}

		set {
			_pathList = value;
			_routeIndex = -1;
		}
	}

	public int RouteIndex {
		get {
			return _routeIndex;
		}
	}
	#endregion

	#region Methods
	// Use this for initialization
	private void Start()
	{
		if (!Mover)
		{
			Mover = gameObject.AddComponent<VehicleMover>();
		}
		Mover.OnArrive += OnArrive;
	}

	protected virtual void OnArrive()
	{
		_routeIndex = (RouteIndex + 1) % _pathList.Count;
		_mover.WayPointList = _pathList[RouteIndex].WayPoints;
	}
	#endregion
}